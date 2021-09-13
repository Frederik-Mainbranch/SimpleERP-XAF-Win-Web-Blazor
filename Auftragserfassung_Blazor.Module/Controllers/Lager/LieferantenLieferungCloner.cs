using System;
using System.Collections.Generic;
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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class LieferantenLieferungCloner : ViewController
    {
        public LieferantenLieferungCloner()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void simpleAction1_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }

        private void parametrizedAction1_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            Session session = ((XPObjectSpace)this.ObjectSpace).Session;
            int anzahl = (int)(e.ParameterCurrentValue);

            for (int i = 0; i < anzahl; i++)
            {
                foreach (LieferantenLieferung alteLieferung in e.SelectedObjects)
                {
                    LieferantenLieferung neueLieferung = new LieferantenLieferung(session);
                    neueLieferung.Lieferant = alteLieferung.Lieferant;
                    neueLieferung.LieferantenLieferscheinnummer = alteLieferung.LieferantenLieferscheinnummer + 1 + i;
                    neueLieferung.LieferantenLieferungWurdeKommittet = true;
                    neueLieferung.Lieferdatum = alteLieferung.Lieferdatum;
                    neueLieferung.Wareneingang_Lager = alteLieferung.Wareneingang_Lager;
                    foreach (LieferantenLieferungPosition altePosition in alteLieferung.LieferantenLieferungPositionenListe)
                    {
                        LieferantenLieferungPosition neuePosi = new LieferantenLieferungPosition(session);
                        neuePosi.LieferantenLieferung = neueLieferung;
                        neuePosi.Artikel = altePosition.Artikel;
                        neuePosi.Lieferant = altePosition.Lieferant;
                        neuePosi.Liefermenge = altePosition.Liefermenge;
                        neuePosi.Positionsnummer = altePosition.Positionsnummer;
                }
                }
            }

            if(ObjectSpace.IsModified == true)
            {
                ObjectSpace.CommitChanges();
            }
            View.Refresh(true);
        }
    }
}
