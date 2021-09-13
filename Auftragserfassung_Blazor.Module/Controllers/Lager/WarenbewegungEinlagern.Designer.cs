namespace Auftragserfassung_Blazor.Module.Controllers
{
    partial class WarenbewegungEinlagern
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Action_WarenbewegungEinlagern = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // Action_WarenbewegungEinlagern
            // 
            this.Action_WarenbewegungEinlagern.Caption = "Action_WarenbewegungEinlagern";
            this.Action_WarenbewegungEinlagern.Category = "Edit";
            this.Action_WarenbewegungEinlagern.ConfirmationMessage = null;
            this.Action_WarenbewegungEinlagern.Id = "Action_WarenbewegungEinlagern";
            this.Action_WarenbewegungEinlagern.ImageName = "BO_Transition";
            this.Action_WarenbewegungEinlagern.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
            this.Action_WarenbewegungEinlagern.TargetObjectType = typeof(Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager.Waren_Bewegung);
            this.Action_WarenbewegungEinlagern.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.Action_WarenbewegungEinlagern.ToolTip = "Warenbewegung ins Lager einbuchen";
            this.Action_WarenbewegungEinlagern.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.Action_WarenbewegungEinlagern.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.Action_WarenbewegungEinlagern_Execute);
            // 
            // WarenbewegungEinlagern
            // 
            this.Actions.Add(this.Action_WarenbewegungEinlagern);
            this.TargetObjectType = typeof(Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager.Waren_Bewegung);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction Action_WarenbewegungEinlagern;
    }
}
