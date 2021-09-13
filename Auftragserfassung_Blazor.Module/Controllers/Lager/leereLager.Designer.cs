namespace Auftragserfassung_Blazor.Module.Controllers
{
    partial class leereLager
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
            this.LagerLeeren = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // LagerLeeren
            // 
            this.LagerLeeren.Caption = "Lager Leeren";
            this.LagerLeeren.Category = "Edit";
            this.LagerLeeren.ConfirmationMessage = null;
            this.LagerLeeren.Id = "LagerLeeren";
            this.LagerLeeren.TargetObjectType = typeof(Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager.Lager);
            this.LagerLeeren.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.LagerLeeren.ToolTip = null;
            this.LagerLeeren.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.LagerLeeren.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.LagerLeeren_Execute);
            // 
            // leereLager
            // 
            this.Actions.Add(this.LagerLeeren);
            this.TargetObjectType = typeof(Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager.Lager);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction LagerLeeren;
    }
}
