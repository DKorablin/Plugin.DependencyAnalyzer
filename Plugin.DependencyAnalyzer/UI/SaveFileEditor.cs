using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer.UI
{
	/// <summary>The UI editor control for save file dialog.</summary>
	public class SaveFileEditor : UITypeEditor
	{
		/// <inheritdoc/>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.Modal;

		/// <inheritdoc/>
		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			if(context == null || context.Instance == null || provider == null)
				return base.EditValue(context, provider, value);

			String filePath = (String)value;
			String directory = filePath == null ? null : Path.GetFullPath(filePath);
			DataObjectSave obj = (DataObjectSave)context.Instance;
			if(filePath == null)
				filePath = ((Data.IDataObject)obj).Name;

			using(SaveFileDialog dlg = new SaveFileDialog()
			{
				OverwritePrompt = true,
				AddExtension = true,
				FileName = filePath,
				InitialDirectory = directory,
				DefaultExt = "." + obj.ImageExtension,
				Filter = "JPG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|Png Files (*.png)|*.png|SVG files (*.svg)|*.svg|Graph file (*.msagl)|*.msagl|All files (*.*)|*.*",
				Title = "Save graph",
			})
				if(dlg.ShowDialog() == DialogResult.OK)
					obj.FilePath = dlg.FileName;//HACK: If the path has not changed, the PropertyGrid will not call the set method.
			return null;
		}
	}
}