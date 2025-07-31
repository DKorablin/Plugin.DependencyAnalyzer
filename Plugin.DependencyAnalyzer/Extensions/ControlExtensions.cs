using System;
using System.Windows.Forms;

namespace Plugin.DependencyAnalyzer.Extensions
{
	internal static class ControlExtensions
	{
		public static void InvokeWithCheck(this Control ctrl, Action action)
		{
			if(ctrl.InvokeRequired)
				ctrl.BeginInvoke(new MethodInvoker(() => action.Invoke()));
			else
				action.Invoke();
		}

		public static T InvokeWithResult<T>(this Control ctrl, Func<T> action)
			=> ctrl.InvokeRequired
				? (T)ctrl.Invoke(action)
				: action.Invoke();
	}
}