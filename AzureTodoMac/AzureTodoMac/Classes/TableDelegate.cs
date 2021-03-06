﻿using System;

using AppKit;
using Foundation;

namespace AzureTodo
{
	/// <summary>
	/// Delegate - controls selection and cell UI layout
	/// </summary>
	public class TableDelegate: NSTableViewDelegate
	{
		#region Private Constants
		const string CellIdentifier = "ProdCell";
		#endregion

		#region Private Variables
		MainWindowController windowController;
		#endregion

		#region Constructors
		public TableDelegate (MainWindowController winctrl)
		{
			windowController = winctrl;
		}
		#endregion

		#region Override Methods
		public override bool ShouldSelectRow (NSTableView tableView, nint row)
		{
			return (row >= 0 && row < tableView.DataSource.GetRowCount (tableView));
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			Console.WriteLine (notification);
			var table = notification.Object as NSTableView;
			var row = table.SelectedRow;

			// Anything to process
			if (row < 0)
				return;

			// Get current values from the data source
			var name = (NSString)table.DataSource.GetObjectValue (table, new NSTableColumn ("name"), row);
			var id = (NSString)table.DataSource.GetObjectValue (table, new NSTableColumn ("id"), row);

			// Confirm deletion of a todo item
			var alert = new NSAlert {
				AlertStyle = NSAlertStyle.Critical,
				InformativeText = string.Format ("Do you want to delete row {0}?", name),
				MessageText = "Delete Todo",
			};
			alert.AddButton ("Cancel");
			alert.AddButton ("Delete");
			alert.BeginSheetForResponse (windowController.Window, async result => {
				Console.WriteLine ("Alert Result: {0}", result);
				if (result == 1001)
					await windowController.Delete (id);
				table.DeselectAll (this);
			});
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
			var view = (NSTextField)tableView.MakeView (CellIdentifier, this);
			if (view == null) {
				view = new NSTextField {
					Identifier = CellIdentifier,
					BackgroundColor = NSColor.Clear,
					Bordered = false,
					Selectable = false,
					Editable = false
				};
			}

			// Grab the data for the given table column
			view.StringValue = (NSString)tableView.DataSource.GetObjectValue (tableView, tableColumn, row);

			// Return the data
			return view;
		}
		#endregion
	}
}

