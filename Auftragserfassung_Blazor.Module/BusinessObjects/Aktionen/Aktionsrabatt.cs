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
using Auftragserfassung_Blazor.Module.Interfaces;
using Auftragserfassung_Blazor.Module.Helpers;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    #region//Klasse
    [DefaultClassOptions]

    public class Aktionsrabatt : Aktion
    {
        public Aktionsrabatt(Session session)
            : base(session)
        {
        }

        #endregion
        #region//Override Methoden


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {

            if (IsLoading == false && IsSaving == false)
            {
                if((propertyName == nameof(AktionsArtikel) && AktionsArtikel != null) || (propertyName == nameof(ArtikelUntergruppe) && ArtikelUntergruppe != null))
                {
                    DarfUeberschriebenWerden = true;
                }
                else if (propertyName == nameof(DarfUeberschriebenWerden) && AktionWurdeCommited == true)
                {
                    UpdateAktuellenRabatt_Chef();
                }
            }
        }


        protected override void OnSaving()
        {
            base.OnSaving(); 
            //Überprüfung, ob Aktion zulässig ist (korrekte Datumseingabe und ob für einen Tag 2 Aktionen gelten in Artikelrabtt sowie in der Gruppe und Untergruppe

            if(IsDeleted == false)
            {
                UpdateAktuellenRabatt_Chef();
            }
        }


        protected override void OnDeleting()
        {
            base.OnDeleting();
            UpdateAktuellenRabatt_Chef();
        }

        #endregion

        #region//Methoden

        private void UpdateAktuellenRabatt_Chef()
        {
            AktionsHelper2000 aktionsHelper = new AktionsHelper2000(Session);

            if (AktionsArtikel != null)
            {
                aktionsHelper.UpdateAktuellenRabatt_Artikel(AktionsArtikel);
                aktionsHelper.UpdateAktuellenPreis(AktionsArtikel);
            }
            else if (ArtikelUntergruppe != null)
            {
                aktionsHelper.UpdateAktuellenRabatt_Untergruppe(ArtikelUntergruppe);

                foreach (Artikel artikel in ArtikelUntergruppe.ArtikelUntergruppeArtikelListe)
                {
                    aktionsHelper.UpdateAktuellenRabatt_Artikel(artikel);
                    aktionsHelper.UpdateAktuellenPreis(artikel);
                }

            }
            else if (ArtikelGruppe != null)
            {
                aktionsHelper.UpdateAktuellenRabatt_Gruppe(ArtikelGruppe);

                foreach (ArtikelUntergruppe artikelUntergruppe in ArtikelGruppe.ArtikelUntergruppenListe)
                {
                    aktionsHelper.UpdateAktuellenRabatt_Untergruppe(artikelUntergruppe);
                    foreach (Artikel artikel in artikelUntergruppe.ArtikelUntergruppeArtikelListe)
                    {
                        aktionsHelper.UpdateAktuellenRabatt_Artikel(artikel);
                        aktionsHelper.UpdateAktuellenPreis(artikel);
                    }
                }
            }
        }


        #endregion

        #region//Properties

        private double _AktionsRabatt;
        [ModelDefault("DisplayFormat", "{0:#0.# '%'}")]
        [ModelDefault("EditMask", "#0.#")]
        public double AktionsRabatt
        {
            get { return _AktionsRabatt; }
            set { SetPropertyValue<double>(nameof(AktionsRabatt), ref _AktionsRabatt, value); }
        }

        private Artikel _AktionsArtikel;
        [Association("Artikel-Aktionsrabatt")]
        [Browsable(false)]
        public Artikel AktionsArtikel
        {
            get { return _AktionsArtikel; }
            set { SetPropertyValue<Artikel>(nameof(AktionsArtikel), ref _AktionsArtikel, value); }
        }


        private ArtikelUntergruppe _ArtikelUntergruppe;
        [Browsable(false)]
        [Association("ArtikelUntergruppe-Aktionsrabatt")]
        public ArtikelUntergruppe ArtikelUntergruppe
        {
            get { return _ArtikelUntergruppe; }
            set { SetPropertyValue<ArtikelUntergruppe>(nameof(ArtikelUntergruppe), ref _ArtikelUntergruppe, value); }
        }


        private ArtikelGruppe _ArtikelGruppe;
        [Browsable(false)]
        [Association("ArtikelGruppe-Aktionsrabatt")]
        public ArtikelGruppe ArtikelGruppe
        {
            get { return _ArtikelGruppe; }
            set { SetPropertyValue<ArtikelGruppe>(nameof(ArtikelGruppe), ref _ArtikelGruppe, value); }
        }


        private bool _DarfueberschriebenWerden;
        [ImmediatePostData]
        public bool DarfUeberschriebenWerden
        {
            get { return _DarfueberschriebenWerden; }
            set { SetPropertyValue<bool>(nameof(DarfUeberschriebenWerden), ref _DarfueberschriebenWerden, value); }
        }



        #endregion

        #region//Non Persistent Properties

        #endregion
    }
}