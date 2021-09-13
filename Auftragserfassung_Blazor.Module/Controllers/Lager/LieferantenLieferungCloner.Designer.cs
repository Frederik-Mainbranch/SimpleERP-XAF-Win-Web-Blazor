namespace Auftragserfassung_Blazor.Module.Controllers
{
    partial class LieferantenLieferungCloner
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
            this.parametrizedAction1 = new DevExpress.ExpressApp.Actions.ParametrizedAction(this.components);
            // 
            // parametrizedAction1
            // 
            this.parametrizedAction1.Caption = "Clone L_Lieferung2";
            this.parametrizedAction1.Category = "Edit";
            this.parametrizedAction1.ConfirmationMessage = null;
            this.parametrizedAction1.Id = "Clone L_Lieferung2";
            this.parametrizedAction1.NullValuePrompt = null;
            this.parametrizedAction1.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.parametrizedAction1.ShortCaption = null;
            this.parametrizedAction1.ToolTip = null;
            this.parametrizedAction1.ValueType = typeof(int);
            this.parametrizedAction1.Execute += new DevExpress.ExpressApp.Actions.ParametrizedActionExecuteEventHandler(this.parametrizedAction1_Execute);
            // 
            // LieferantenLieferungCloner
            // 
            this.Actions.Add(this.parametrizedAction1);
            this.TargetObjectType = typeof(Auftragserfassung_Blazor.Module.BusinessObjects.LieferantenLieferung);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;

        }

        #endregion
        private DevExpress.ExpressApp.Actions.ParametrizedAction parametrizedAction1;
    }
}
