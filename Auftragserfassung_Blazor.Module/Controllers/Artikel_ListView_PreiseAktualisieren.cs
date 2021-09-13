using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using Auftragserfassung_Blazor.Module.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraEditors;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    public partial class PreiseAktualisieren : ViewController
    {
        SimpleAction Action_PreiseAkualisieren;
        public PreiseAktualisieren()
        {
            InitializeComponent();
            TargetObjectType = typeof(Artikel);
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Root;

            Action_PreiseAkualisieren = new SimpleAction(this, "Preise aktualisieren", PredefinedCategory.View);
            Action_PreiseAkualisieren.ImageName = "Currency";
            Action_PreiseAkualisieren.Execute += verwendetePreiseAkualisieren_Execute;
        }


        private void verwendetePreiseAkualisieren_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Session session = ((XPObjectSpace)this.ObjectSpace).Session;
            AktionsHelper2000 aktionsHelper = new AktionsHelper2000(session);
            aktionsHelper.UpdateAlleAktuelleSteuern(); //Steuern


            ObjectSpace.CommitChanges();
            View.Refresh(true);
            //verwendeteSteuerErmittler verwendeteSteuerErmittler = new verwendeteSteuerErmittler(session);

            //verwendeteSteuerErmittler.AktualisiereFürAlleArtikel_VerwendeteSteuer();
            //verwendeteSteuerErmittler.AktualisiereFürAlleArtikel_VerwendeterPreis();
            //verwendeteSteuerErmittler.AktualisiereFürAlleArtikel_VerwendeterRabatt();

            //XtraMessageBox.Show("leere Action...");
        }
    }
}
