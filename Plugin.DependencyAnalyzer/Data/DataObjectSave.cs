using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Plugin.DependencyAnalyzer.UI;

namespace Plugin.DependencyAnalyzer.Data
{
	internal class DataObjectSave : IDataObject
	{
		public enum ImageFormatType
		{
			/// <summary>bmp</summary>
			Bmp,
			/// <summary>gif</summary>
			Gif,
			/// <summary>jpeg</summary>
			Jpeg,
			/// <summary>png</summary>
			Png,
			/// <summary>msagl</summary>
			Graph,
			/// <summary>svg</summary>
			Svg,
			/// <summary>emf</summary>
			Emf,
			/// <summary>wmf</summary>
			Wmf,
		}

		private readonly DocumentDependencies _form;
		private readonly GViewer _control;
		private String _filePath;
		private Double _imageScale = 1.0;

		#region UI
		/// <summary>Save area to file</summary>
		public enum SaveViewType
		{
			/// <summary>Save entry diagram, not only visible type</summary>
			Global,
			/// <summary>Save only visible part</summary>
			Visible,
		}

		String IDataObject.Name
		{
			get
			{
				LibraryAnalyzer analyzer = this._form.Analyzer;
				return analyzer == null || analyzer.StartLibrary == null
					? null
					: analyzer.StartLibrary.Assembly.Name;
			}
		}

		[DefaultValue(SaveViewType.Global)]
		[Description("Save area to file:\r\nGlobal - Entry diagram\r\nVisible - Visible part")]
		public SaveViewType View { get; set; } = SaveViewType.Global;

		[DefaultValue(PixelFormat.Format32bppPArgb)]
		[Description("The pixel format for the new System.Drawing.Bitmap. This must specify a value that begins with Format")]
		public PixelFormat Format { get; set; } = PixelFormat.Format32bppPArgb;

		[DefaultValue(ImageFormatType.Jpeg)]
		[Description("Image format type")]
		public ImageFormatType ImageFormat { get; set; } = ImageFormatType.Jpeg;

		internal String ImageExtension
		{
			get => ImageFormatToExtension(this.ImageFormat);
			set => this.ImageFormat = ImageFormatFromExtension(value);
		}

		[DefaultValue(1.0)]
		[Editor(typeof(RangeEditor), typeof(UITypeEditor))]
		[DisplayName("Image scale")]
		[Description("Scale of resulted image")]
		public Double ImageScale
		{
			get => this._imageScale;
			set => this._imageScale = value >= 1.0 && value <= 10
					? value
					: this._imageScale;
		}

		[Editor(typeof(SaveFileEditor), typeof(UITypeEditor))]
		[DisplayName("File path")]
		[Description("File path to save result image. Fter selection file will be saved")]
		public String FilePath
		{
			get => this._filePath;
			set
			{
				if(value == null)
					return;

				this.ImageFormat = DataObjectSave.ImageFormatFromExtension(Path.GetExtension(value).ToLowerInvariant());
				this._filePath = value;
				this.SaveFile();
				this._form.tsbnSave_Click(this, EventArgs.Empty);
			}
		}
		#endregion UI

		#region Reflection
		private RectangleF SrcRect
			=> (RectangleF)this._control.GetType().InvokeMember("SrcRect", BindingFlags.GetProperty | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance, null, this._control, null);

		private Boolean SaveCurrentViewInImage
		{
			get => (Boolean)this._control.GetType().InvokeMember("SaveCurrentViewInImage", BindingFlags.GetProperty | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance, null, this._control, null);
			set => this._control.GetType().InvokeMember("SaveCurrentViewInImage", BindingFlags.SetProperty | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance, null, this._control, new Object[] { value, });
		}

		private Object DGraph
			=> this._control.GetType().InvokeMember("DGraph", BindingFlags.GetProperty | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance, null, this._control, null);

		private Matrix CurrentTransform()
			=> (Matrix)this._control.GetType().InvokeMember("CurrentTransform", BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance, null, this._control, null);
		#endregion Reflection

		public DataObjectSave(DocumentDependencies form, GViewer control)
		{
			this._form = form;
			this._control = control;

			this.View = this.SaveCurrentViewInImage ? SaveViewType.Visible : SaveViewType.Global;
		}

		public void SaveFile()
		{
			Size size = this.GetScale();
			ImageFormatType formatType = this.ImageFormat;
			switch(formatType)
			{
			case ImageFormatType.Graph://Записываем в UserData источник бинарника, ибо Object не сериализуется. TODO: Либо оформить LibraryAnalyzer->SerializableAttribute
				LibraryAnalyzer analyzer = (LibraryAnalyzer)this._control.Graph.UserData;
					this._control.Graph.UserData = analyzer.StartLibrary.Path;
					this._control.Graph.Write(this.FilePath);
					this._control.Graph.UserData = analyzer;
					break;
			case ImageFormatType.Emf:
			case ImageFormatType.Wmf:
				File.WriteAllBytes(this.FilePath, this.DrawVectorGraphics(size));
				break;
			case ImageFormatType.Svg:
				SvgGraphWriter.Write(this._control.Graph, this.FilePath, null, null, 4);
				break;
			default://Image
				File.WriteAllBytes(this.FilePath, this.DrawBitmap(size, formatType, this.Format));
				break;
			}
		}

		private Size GetScale()
			=> this.GetScale(this.ImageScale);

		internal Size GetScale(Double scale)
		{
			Int32 width;
			Int32 height;
			switch(this.View)
			{
			case SaveViewType.Visible:
				width = (Int32)Math.Ceiling((Double)this.SrcRect.Width * scale);
				height = (Int32)Math.Ceiling((Double)this.SrcRect.Height * scale);
				break;
			default:
				width = (Int32)Math.Ceiling(this._control.Graph.Width * scale);
				height = (Int32)Math.Ceiling(this._control.Graph.Height * scale);
				break;
			}

			return new Size(width, height);
		}

		private void DrawGeneral(Size size, Graphics graphics)
		{
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

			switch(this.View)
			{
			case SaveViewType.Visible:
				DrawCurrent(graphics);
				break;
			default:
				DrawAll(size, graphics);
				break;
			}
		}

		private void DrawCurrent(Graphics graphics)
		{
			graphics.Transform = this.CurrentTransform();
			graphics.FillRectangle(new SolidBrush(Draw.MsaglColorToDrawingColor(this._control.Graph.Attr.BackgroundColor)), this.SrcRect);
			graphics.Clip = new Region(this.SrcRect);
			Draw.DrawPrecalculatedLayoutObject(graphics, this.DGraph);
		}

		private void DrawAll(Size size, Graphics graphics)
		{
			graphics.FillRectangle(new SolidBrush(Draw.MsaglColorToDrawingColor(this._control.Graph.Attr.BackgroundColor)), new RectangleF(0f, 0f, size.Width, size.Height));
			Double scale = this.ImageScale;
			Graph graph = this._control.Graph;

			Double num2 = 0.5 * (Double)size.Width - scale * (graph.Left + 0.5 * graph.Width);
			Double num3 = 0.5 * (Double)size.Height + scale * (graph.Bottom + 0.5 * graph.Height);
			graphics.Transform = new Matrix((Single)scale, 0f, 0f, (Single)(0.0 - scale), (Single)num2, (Single)num3);
			Draw.DrawPrecalculatedLayoutObject(graphics, this.DGraph);
		}

		private Byte[] DrawVectorGraphics(Size size)
		{
			using(Graphics graphics = this._form.CreateGraphics())
				using(MemoryStream stream = new MemoryStream())
				{
					IntPtr hdc = graphics.GetHdc();
					Metafile image = new Metafile(stream, hdc, EmfType.EmfOnly);
					graphics.ReleaseHdc(hdc);
					using(Graphics gImage = Graphics.FromImage(image))
						this.DrawGeneral(size, gImage);

					return stream.ToArray();
				}
		}

		/// <summary>Сохранить картинку в буфер обмена</summary>
		public void SaveBitmatToClipboard()
		{
			Size size = this.GetScale();
			using(Bitmap bitmap = new Bitmap(size.Width, size.Height, this.Format))
			{
				using(Graphics graphics = Graphics.FromImage(bitmap))
					this.DrawGeneral(bitmap.Size, graphics);
				Clipboard.SetImage(bitmap);
			}
		}

		private Byte[] DrawBitmap(Size size, ImageFormatType type, PixelFormat pixel)
		{
			using(Bitmap bitmap = new Bitmap(size.Width, size.Height, pixel))
			{
				using(Graphics graphics = Graphics.FromImage(bitmap))
					this.DrawGeneral(bitmap.Size, graphics);

				using(MemoryStream stream = new MemoryStream())
				{
					bitmap.Save(stream, DataObjectSave.ImageFormatFromType(type));
					return stream.ToArray();
				}
			}
		}

		private static ImageFormatType ImageFormatFromExtension(String extension)
		{
			switch(extension)
			{
			case ".jpg": return ImageFormatType.Jpeg;
			case ".bmp": return ImageFormatType.Bmp;
			case ".gif": return ImageFormatType.Gif;
			case ".svg": return ImageFormatType.Svg;
			case ".emf": return ImageFormatType.Emf;
			case ".wmf": return ImageFormatType.Wmf;
			case ".msagl": return ImageFormatType.Graph;
			case ".png":
			default: return ImageFormatType.Png;
			}
		}

		private static ImageFormat ImageFormatFromType(ImageFormatType type)
		{
			switch(type)
			{
			case ImageFormatType.Bmp: return System.Drawing.Imaging.ImageFormat.Bmp;
			case ImageFormatType.Gif: return System.Drawing.Imaging.ImageFormat.Gif;
			case ImageFormatType.Jpeg: return System.Drawing.Imaging.ImageFormat.Jpeg;
			default:
			case ImageFormatType.Png: return System.Drawing.Imaging.ImageFormat.Png;
			}
		}

		private static String ImageFormatToExtension(ImageFormatType type)
		{
			switch(type)
			{
			case ImageFormatType.Bmp: return ".bmp";
			case ImageFormatType.Emf: return ".emf";
			case ImageFormatType.Gif: return ".gif";
			case ImageFormatType.Graph: return ".msagl";
			case ImageFormatType.Jpeg: return ".jpg";
			case ImageFormatType.Png: return ".png";
			case ImageFormatType.Svg: return ".svg";
			case ImageFormatType.Wmf: return ".wmf";
			default: throw new NotImplementedException(type.ToString());
			}
		}
	}
}