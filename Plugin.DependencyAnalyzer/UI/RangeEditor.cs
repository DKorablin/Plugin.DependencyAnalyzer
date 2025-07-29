using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Plugin.DependencyAnalyzer.Data;

namespace Plugin.DependencyAnalyzer.UI
{
	/// <summary>The UI type editor for ranged objects.</summary>
	public class RangeEditor : UITypeEditor
	{
		/// <inheritdoc/>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown;

		/// <inheritdoc/>
		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if(editorService != null)
			{
				RangeEditorCtrl ctrl = new RangeEditorCtrl((DataObjectSave)context.Instance);

				editorService.DropDownControl(ctrl);
				value = ctrl.Value;
			}
			return value;
		}
	}
}