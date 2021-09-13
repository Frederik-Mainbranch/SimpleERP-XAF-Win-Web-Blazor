using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Xpo.DB;
using Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(LieferantenLieferscheinnummer))]
    public class LieferantenLieferung : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public LieferantenLieferung(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Lieferdatum = DateTime.Today;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if(IsDeleted == false && IsLoading == false && IsSaving == false)
            {
                if(propertyName == nameof(Lieferant))
                {
                    if((Lieferant)oldValue == null)
                    {
                        foreach (LieferantenLieferungPosition lieferungPosition in LieferantenLieferungPositionenListe)
                        {
                            lieferungPosition.Lieferant = Lieferant;
                        }
                    }
                    else if((Lieferant)oldValue != null && (Lieferant)newValue != null)
                    {
                        foreach (LieferantenLieferungPosition lieferungPosition in LieferantenLieferungPositionenListe)
                        {
                            lieferungPosition.Lieferant = Lieferant;
                            lieferungPosition.Artikel = null;
                            lieferungPosition.Liefermenge = 0;
                        }
                    }
                    else if((Lieferant)oldValue != null && (Lieferant)newValue == null)
                    {
                        foreach (LieferantenLieferungPosition lieferungPosition in LieferantenLieferungPositionenListe)
                        {
                            lieferungPosition.Lieferant = null;
                            lieferungPosition.Artikel = null;
                            lieferungPosition.Liefermenge = 0;
                        }
                    }
                }

                else if(propertyName == nameof(BelegWurdeCommitted) && BelegWurdeCommitted == true && LieferantenLieferscheinnummer < 1)
                {
                    SelectedData MaxnummerL = Session.ExecuteQuery("SELECT MAX(LieferantenLieferscheinnummer) FROM LieferantenLieferung");
                    if ((MaxnummerL.ResultSet[0].Rows[0].Values[0] != null))
                    {
                        LieferantenLieferscheinnummer = int.Parse(MaxnummerL.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
                    }
                    else
                    {
                        LieferantenLieferscheinnummer = 1;
                    }
                }
            }
        }


        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Methoden ---------------------------------------------



        //-------------------------------- Methoden ---------------------------------------------
        //-------------------------------- Properties ---------------------------------------------

        private bool _BelegWurdeCommitted;
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [ImmediatePostData]
        public bool BelegWurdeCommitted
        {
            get { return _BelegWurdeCommitted; }
            set { SetPropertyValue<bool>(nameof(BelegWurdeCommitted), ref _BelegWurdeCommitted, value); }
        }


        private Lager _Wareneingang_Lager;
        [RuleRequiredField]
        [DataSourceCriteria("Wareneingang==true")]
        public Lager Wareneingang_Lager
        {
            get { return _Wareneingang_Lager; }
            set { SetPropertyValue<Lager>(nameof(Wareneingang_Lager), ref _Wareneingang_Lager, value); }
        }


        private int _LieferantenLieferscheinnummer;
        [ModelDefault("AllowEdit", "false")]
        [Appearance("LieferantenLieferscheinnummer", Criteria = nameof(LieferantenLieferscheinnummer) + " = 0", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public int LieferantenLieferscheinnummer
        {
            get { return _LieferantenLieferscheinnummer; }
            set { SetPropertyValue<int>(nameof(LieferantenLieferscheinnummer), ref _LieferantenLieferscheinnummer, value); }
        }


        private DateTime _Lieferdatum;
        [RuleRequiredField]
        [ModelDefault("EditMask", "{0:#0.# }")]
        public DateTime Lieferdatum
        {
            get { return _Lieferdatum; }
            set { SetPropertyValue<DateTime>(nameof(Lieferdatum), ref _Lieferdatum, value); }
        }


        private bool _LieferantenLieferungWurdeKommittet;
        [Browsable(false)]
        public bool LieferantenLieferungWurdeKommittet
        {
            get { return _LieferantenLieferungWurdeKommittet; }
            set { SetPropertyValue<bool>(nameof(LieferantenLieferungWurdeKommittet), ref _LieferantenLieferungWurdeKommittet, value); }
        }



        private Lieferant _Lieferant;
        [Association("Lieferant-LieferantenLieferung")]
        [ImmediatePostData]
        public Lieferant Lieferant
        {
            get { return _Lieferant; }
            set { SetPropertyValue<Lieferant>(nameof(Lieferant), ref _Lieferant, value); }
        }






        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------


        [DevExpress.Xpo.Aggregated, Association("LieferantenLieferung-LieferantenLieferungPosition")]
        public XPCollection<LieferantenLieferungPosition> LieferantenLieferungPositionenListe
        {
            get { return GetCollection<LieferantenLieferungPosition>(nameof(LieferantenLieferungPositionenListe)); }
        }


        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------



        //-------------------------------- Non Persistent Properties ---------------------------------------------






    }
}