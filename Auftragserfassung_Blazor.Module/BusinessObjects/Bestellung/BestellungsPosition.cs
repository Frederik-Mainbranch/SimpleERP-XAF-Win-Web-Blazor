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
using System.ComponentModel.DataAnnotations.Schema;
using DisplayNameAttribute = DevExpress.Xpo.DisplayNameAttribute;
using Auftragserfassung_Blazor.Module.Helpers;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]


    public class BestellungsPosition : BelegPosition
    {
        public BestellungsPosition(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false && Bestellung != null)
            {
                if (propertyName == nameof(Artikel) && Artikel != null)
                {
                    Artikel_Preis = Artikel.AktuellerPreis;
                    Artikel_Steuersatz = Artikel.Steuersatz.AktuellerSteuersatz;

                    if (Positionsnummer == 0)    //bestimmt bei neu angelegten Positionen die neue Positionsnummer
                    {
                        int positionsnummer = 0;
                        foreach (BestellungsPosition position in Bestellung.BestellungsPositionenListe)
                        {
                            if (position.Artikel != null)
                            {
                                positionsnummer++;
                            }
                        }
                        Positionsnummer = positionsnummer;
                    }
                }
                else if (propertyName == nameof(Artikel_Preis) || propertyName == nameof(AnzahlBestellteMenge) || propertyName == nameof(Zeilenrabatt))
                {
                    if (Artikel != null)
                    {
                        Bestellung.BildeRechnungsSummen();
                    }
                }
            }
        }


        protected override void OnDeleting()
        {
            base.OnDeleting();
            AnzahlBestellteMenge = 0; //löst die OnChanged Methode aus, welche die neue Rechnungssumme bildet

            //Positionsnummer
            foreach (BestellungsPosition position in Bestellung.BestellungsPositionenListe)
            {
                if(position.Positionsnummer > Positionsnummer)
                {
                    position.Positionsnummer--; 
                }
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if(IsDeleted == false && PositionWurdeCommited == false)
            {
                Artikel.AnzahlReserviert += AnzahlBestellteMenge;
                Artikel.ReserviereArtikelNachBestellung(AnzahlBestellteMenge);
                ZugehörigeBestellungWurdeCommitted = true;
            }
        }

        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Methoden ---------------------------------------------


        #region   //alt     
        //public void BesorgeArtikelAktionsDaten() //Debug
        //{
        //    AktionspreisBestellterArtikel = 0;
        //    AktionsrabattBestellterArtikel = 0;

        //    //Ermittlung des Aktionspreises, wenn es einen gibt
        //    if(Bestellung.BenutzeAktionspreis == true)
        //    {
        //        foreach (Aktionspreis aktPreis in Artikel.AktionspreiseListe)
        //        {
        //            if (RelevantesDatum > aktPreis.AktionStart && RelevantesDatum < aktPreis.AktionEnde)
        //            {
        //                AktionspreisBestellterArtikel = aktPreis.AktionPreis;
        //                break;
        //            }
        //        }
        //    }


        //    //Ermittlung des Aktionsrabattes, wenn es einen gibt
        //    if(Bestellung.BenutzeAktionsrabatt == true && Artikel != null)
        //    {
        //        foreach (Aktionsrabatt aktRabatt in Artikel.AktionsrabatteListe)
        //        {
        //            if (RelevantesDatum > aktRabatt.AktionStart && RelevantesDatum < aktRabatt.AktionEnde)
        //            {
        //                AktionsrabattBestellterArtikel = aktRabatt.AktionsRabatt;
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        AktionsrabattBestellterArtikel = 0;
        //    }
        //}


        //private void BesorgeArtikelSteuer()
        //{
        //    //Ermittlung der Aktionssteuer, wenn es eine gibt
        //    verwendeteSteuerErmittler verwendeteSteuerErmittler = new verwendeteSteuerErmittler(Session);
        //    BestellterArtikelSteuer = verwendeteSteuerErmittler.ErmittlePositionsSteuer(Artikel, RelevantesDatum);


        //    //bool aktionssteuerGefunden = false;

        //    //foreach (vorübergehendeSteuer vorübergehendeSteuer in Artikel.Steuersatz.vorübergehendeSteuerListe)
        //    //{
        //    //    if (RelevantesDatum > vorübergehendeSteuer.AktionStart && RelevantesDatum < vorübergehendeSteuer.AktionEnde)
        //    //    {
        //    //        BestellterArtikelSteuer = vorübergehendeSteuer.AktionsSteuer;
        //    //        aktionssteuerGefunden = true;
        //    //        break;
        //    //    }
        //    //}


        //    //if (aktionssteuerGefunden == false)
        //    //{
        //    //    BestellterArtikelSteuer = Artikel.verwendeteSteuer;
        //    //} 

        //}
        #endregion

        //-------------------------------- Methoden ---------------------------------------------
        //------------------------------- Properties ----------------------------------------------


        private Bestellung _Bestellung;
        [Association("Bestellung-BestellungsPosition")]
        [VisibleInDashboards(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public Bestellung Bestellung
        {
            get { return _Bestellung; }
            set { SetPropertyValue<Bestellung>(nameof(Bestellung), ref _Bestellung, value); }
        }


        private bool _ZugehörigeBestellungWurdeKommittet;
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public bool ZugehörigeBestellungWurdeCommitted
        {
            get { return _ZugehörigeBestellungWurdeKommittet; }
            set { SetPropertyValue<bool>(nameof(ZugehörigeBestellungWurdeCommitted), ref _ZugehörigeBestellungWurdeKommittet, value); }
        }


        private bool _PositionIstVollständig;
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public bool PositionIstVollständig
        {
            get { return _PositionIstVollständig; }
            set { SetPropertyValue<bool>(nameof(PositionIstVollständig), ref _PositionIstVollständig, value); }
        }
    }
}