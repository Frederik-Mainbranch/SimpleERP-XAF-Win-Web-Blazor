using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using System.Drawing;
using DevExpress.Utils.Svg;
using DevExpress.Utils;
using DevExpress.WebUtils;

namespace Auftragserfassung_Blazor.Module.Win.Controllers
{
    public partial class VC_Kunde_Win : ViewController
    {
        public VC_Kunde_Win()
        {
            InitializeComponent();
            TargetObjectType = typeof(Kunde);
            TargetViewType = ViewType.DetailView;
        }


        protected override void OnActivated()
        {
            base.OnActivated();
            //View.ObjectSpace.Committing += ObjectSpace_Committing;
            View.ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
        }


        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            //View.ObjectSpace.Committing -= ObjectSpace_Committing;
            View.ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
        }

        //public virtual void ObjectSpace_Committing(object sender, CancelEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (((string)e.PropertyName) == nameof(Kunde.XtraMessage_np) && ((Kunde)e.Object).XtraMessage_np != "")
            {
                XtraMessageBox.Show(((Kunde)e.Object).XtraMessage_np);
                ((Kunde)e.Object).XtraMessage_np = "";
            }
        }
    }
}
