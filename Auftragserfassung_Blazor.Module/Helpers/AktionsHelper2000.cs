using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auftragserfassung_Blazor.Module.Helpers
{
    class AktionsHelper2000
    {
        public AktionsHelper2000(Session session)
        {
            AktiveSession = session;
            StandardSteuer = ErmittleStandardSteuer();
        }
        private Session AktiveSession { get; set; }
        public double StandardSteuer { get; set; }

        private double ErmittleStandardSteuer()
        {
            CriteriaOperator criteria = new BinaryOperator("IstStandard", "True");
            Steuer steuer = (Steuer)AktiveSession.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,typeof(Steuer), criteria);
            if(steuer != null)
            {
                return steuer.StandardSteuersatz;
            }
            else
            {
                NullOperator no = new NullOperator();
                if(AktiveSession.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, typeof(Steuer), no) == null)
                {
                    Steuer standardSteuer = new Steuer(AktiveSession);
                    standardSteuer.StandardSteuersatz = 19;
                    standardSteuer.Bezeichnung = "Standard Steuersatz";
                    standardSteuer.IstStandard = true;

                    Steuer ermaessigteSteuer = new Steuer(AktiveSession);
                    ermaessigteSteuer.StandardSteuersatz = 7;
                    ermaessigteSteuer.Bezeichnung = "Ermäßigter Steuersatz";
                    ermaessigteSteuer.IstStandard = false;

                    return standardSteuer.StandardSteuersatz;
                }
                else
                {
                    throw new UserFriendlyException("Es konnte kein Standardsteuersatz ermittelt werden!");
                }
            }
        }


        public void UpdateAlleAktuellePreise()
        {
            XPCollection<Artikel> vorhandeneArtikel = new XPCollection<Artikel>(AktiveSession);
            foreach (Artikel artikel in vorhandeneArtikel)
            {
                UpdateAktuellenPreis(artikel);
            }
        }

        public void UpdateAktuellenRabatt_Gruppe(ArtikelGruppe artikelGruppe)
        {
            foreach (Aktionsrabatt aktionsrabatt_gruppe in artikelGruppe.AktionsRabattListe)
            {
                if (DateTime.Today >= aktionsrabatt_gruppe.AktionStart && DateTime.Today <= aktionsrabatt_gruppe.AktionEnde && aktionsrabatt_gruppe.AktionWurdeGeloescht_NP == false)
                {
                    artikelGruppe.AktuellerRabatt = aktionsrabatt_gruppe.AktionsRabatt;
                    return;
                }
            }

            artikelGruppe.AktuellerRabatt = 0;
        }

        public void UpdateAktuellenRabatt_Untergruppe(ArtikelUntergruppe artikelUntergruppe)
        {
            bool rabattGefunden = false;
            foreach (Aktionsrabatt aktionsrabatt_untergruppe in artikelUntergruppe.AktionsrabattListe)
            {
                if (DateTime.Today >= aktionsrabatt_untergruppe.AktionStart && DateTime.Today <= aktionsrabatt_untergruppe.AktionEnde && aktionsrabatt_untergruppe.AktionWurdeGeloescht_NP == false)
                {
                    artikelUntergruppe.AktuellerRabatt = aktionsrabatt_untergruppe.AktionsRabatt;
                    artikelUntergruppe.AngewandterRabatt = "Rabatt der Artikeluntergruppe";
                    rabattGefunden = true;

                    if (aktionsrabatt_untergruppe.DarfUeberschriebenWerden == false)
                    {
                        return;
                    }
                    break;
                }
            }


            foreach (Aktionsrabatt aktionsrabatt_gruppe in artikelUntergruppe.ArtikelGruppe.AktionsRabattListe)
            {
                if (DateTime.Today >= aktionsrabatt_gruppe.AktionStart && DateTime.Today <= aktionsrabatt_gruppe.AktionEnde
                    && aktionsrabatt_gruppe.AktionWurdeGeloescht_NP == false && artikelUntergruppe.BenutzteGruppenRabatte == true)
                {
                    artikelUntergruppe.AktuellerRabatt = aktionsrabatt_gruppe.AktionsRabatt;
                    artikelUntergruppe.AngewandterRabatt = $"Rabatt der Artikelgruppe {artikelUntergruppe.ArtikelGruppe.Bezeichnung}";
                    return;
                }
            }

            if (rabattGefunden == false)
            {
                artikelUntergruppe.AktuellerRabatt = 0;
                artikelUntergruppe.AngewandterRabatt = "";
            }
        }

        public void UpdateAktuellenRabatt_Artikel(Artikel artikel)
        {
            #region Logik
            /* Überprüfung von Artikel.Aktionsrabatt, ob es einen gültigen Rabatt gibt. 
             * Wenn es einen gibt, wird dieser gesetzt und überprüft, ob dieser von der Untergruppe überschrieben werden darf
             * wenn ja wird der Aktionsrabatt der Untergruppe gesucht und überprüft, ob dieser überschrieben werden darf
             * wenn er überschrieben werden darf, wird der Aktionsrabatt der Gruppe gesucht und wenn vorhanden ausgegeben
             */
            #endregion

            bool rabattGefunden = false;

            foreach (Aktionsrabatt aktionsrabatt_artikel in artikel.AktionsrabatteListe)
            {
                if (DateTime.Today >= aktionsrabatt_artikel.AktionStart && DateTime.Today <= aktionsrabatt_artikel.AktionEnde && aktionsrabatt_artikel.AktionWurdeGeloescht_NP == false)
                {
                    artikel.AktuellerRabatt = aktionsrabatt_artikel.AktionsRabatt;
                    artikel.AngewandterRabatt = "Rabatt des Artikels";
                    rabattGefunden = true;

                    if (aktionsrabatt_artikel.DarfUeberschriebenWerden == false)
                    {
                        return;
                    }
                    break;
                }
            }


            foreach (Aktionsrabatt aktionsrabatt_untergruppe in artikel.ArtikelUntergruppe.AktionsrabattListe)
            {
                if (DateTime.Today >= aktionsrabatt_untergruppe.AktionStart && DateTime.Today <= aktionsrabatt_untergruppe.AktionEnde
                    && aktionsrabatt_untergruppe.AktionWurdeGeloescht_NP == false && artikel.BenutzeUntergruppenRabatte == true)
                {
                    artikel.AktuellerRabatt = aktionsrabatt_untergruppe.AktionsRabatt;
                    artikel.AngewandterRabatt = $"Rabatt der Artikeluntergruppe {artikel.ArtikelUntergruppe.Bezeichnung}";
                    rabattGefunden = true;

                    if (aktionsrabatt_untergruppe.DarfUeberschriebenWerden == false)
                    {
                        return;
                    }
                    break;
                }
            }


            foreach (Aktionsrabatt aktionsrabatt_gruppe in artikel.ArtikelGruppe.AktionsRabattListe)
            {
                if (DateTime.Today >= aktionsrabatt_gruppe.AktionStart && DateTime.Today <= aktionsrabatt_gruppe.AktionEnde
                    && aktionsrabatt_gruppe.AktionWurdeGeloescht_NP == false && artikel.BenutzteGruppenRabatte == true
                    && artikel.ArtikelUntergruppe.BenutzteGruppenRabatte == true)
                {
                    artikel.AktuellerRabatt = aktionsrabatt_gruppe.AktionsRabatt;
                    artikel.AngewandterRabatt = $"Rabatt der Artikelgruppe {artikel.ArtikelGruppe.Bezeichnung}";
                    rabattGefunden = true;

                    return;
                }
            }

            //Wenn nirgendwo ein aktuell gültiger Rabatt gefunden wurde
            if (rabattGefunden == false)
            {
                artikel.AktuellerRabatt = 0;
                artikel.AngewandterRabatt = "";
            }
        }


        public void UpdateAktuellenPreis(Artikel artikel)
        {
            //Überprüfung, ob es einen Aktionspreis gibt, wenn ja, wird mit diesen weitergerechnet, wenn nein, dann der Standard Preis genommen
            double aktuellerPreis = artikel.StandardPreis;
            foreach (Aktionspreis aktionspreis in artikel.AktionspreiseListe)
            {
                if (DateTime.Today >= aktionspreis.AktionStart && DateTime.Today <= aktionspreis.AktionEnde && aktionspreis.AktionPreis != -1)
                {
                    aktuellerPreis = aktionspreis.AktionPreis;
                    break;
                }
            } //an dieser Stelle ist der aktueller Preis entweder der Standard Preis oder der Aktionspreis

            aktuellerPreis *=  (1 + artikel.AktuellerRabatt / 100);
            artikel.AktuellerPreis = Math.Round(aktuellerPreis, 4);
        }


        public void UpdateAlleAktuelleSteuern()
        {
            XPCollection<Steuer> vorhandeneSteuern = new XPCollection<Steuer>(AktiveSession);
            foreach (Steuer steuer in vorhandeneSteuern)
            {
                UpdateAktuelleSteuer(steuer);
            }
        }


        public void UpdateAktuelleSteuer(Steuer steuer)
        {
            foreach (VoruebergehendeSteuer vorübergehendeSteuer in steuer.VoruebergehendeSteuerListe)
            {
                if (DateTime.Today >= vorübergehendeSteuer.AktionStart && DateTime.Today <= vorübergehendeSteuer.AktionEnde)
                {//Wenn für den heutigen Tag eine Aktionssteuer gilt, wird die Aktuelle Steuer auf den Wert der Aktions Steuer gesetzt
                    steuer.AktuellerSteuersatz = vorübergehendeSteuer.AktionsSteuer;
                    return;
                }
            }

            //wenn keine Aktionssteuer gefunden wurde
            steuer.AktuellerSteuersatz = steuer.StandardSteuersatz;
        }



        #region //alt

        //public void SetzteAktuelleSteuer(Artikel artikel)
        //{

        //    //Überprüfung, ob Untergruppe oder Gruppe eine Aktionssteuer hat


        //    if (artikel.Steuersatz != null)
        //    {
        //        if (artikel.Steuersatz.StandardSteuersatz != 0 || artikel.Steuersatz.IstSteuerfrei == true)
        //        {
        //            //Überprüfen, ob der Steuersatz des Artikels eine Aktionssteuer hat
        //            foreach (VoruebergehendeSteuer vorübergehendeSteuer in artikel.Steuersatz.VoruebergehendeSteuerListe)
        //            {
        //                if (DateTime.Today > vorübergehendeSteuer.AktionStart && DateTime.Today < vorübergehendeSteuer.AktionEnde)
        //                {
        //                    artikel.Steuersatz = vorübergehendeSteuer.Steuer;
        //                    return;
        //                }
        //            }

        //            artikel.Steuersatz.StandardSteuersatz;
        //            return;
        //        }
        //    }

        //    if (artikel.ArtikelUntergruppe != null)
        //    {
        //        if (artikel.ArtikelUntergruppe.Steuersatz != null)
        //        {
        //            if (artikel.ArtikelUntergruppe.Steuersatz.StandardSteuersatz != 0 || artikel.ArtikelUntergruppe.Steuersatz.IstSteuerfrei == true)
        //            {
        //                return artikel.ArtikelUntergruppe.Steuersatz.StandardSteuersatz;
        //            }
        //        }
        //        else if (artikel.ArtikelUntergruppe.ArtikelGruppe != null)
        //        {
        //            if (artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz != null)
        //            {
        //                if (artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.StandardSteuersatz != 0 || artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.IstSteuerfrei == true)
        //                {
        //                    return artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.StandardSteuersatz;
        //                }
        //            }
        //        }
        //    }

        //    //Standard Wert der Steuer, wenn keine andere gefunden wurde
        //    return standardSteuer;
        //}

        //public double ErmittlePositionsSteuer(Artikel artikel, DateTime relevantesDatum)
        //{
        //    //Überprüfung, ob es eine Aktionssteuer gibt
        //    foreach (vorübergehendeSteuer vorübergehendeSteuer in artikel.Steuersatz.vorübergehendeSteuerListe)
        //    {
        //        if (relevantesDatum > vorübergehendeSteuer.AktionStart && relevantesDatum < vorübergehendeSteuer.AktionEnde)
        //        {
        //            return vorübergehendeSteuer.AktionsSteuer;
        //        }
        //    }

        //    //Wenn es keine gibt, wird die artikel/verwendeteSteuer verwendet
        //    return artikel.verwendeteSteuer;
        //}

        //public double ErmittleVerwendeteSteuer(Artikel artikel)
        //{
        //    //Überprüfung, ob Artikel, Untergruppe oder Gruppe eine Aktionssteuer hat

        //    if (artikel.Steuersatz != null)
        //    {
        //        if (artikel.Steuersatz.StandardSteuersatz != 0 || artikel.Steuersatz.IstSteuerfrei == true)
        //        {
        //            return artikel.Steuersatz.StandardSteuersatz;
        //        }
        //    }

        //    if (artikel.ArtikelUntergruppe != null)
        //    {
        //        if (artikel.ArtikelUntergruppe.Steuersatz != null)
        //        {
        //            if (artikel.ArtikelUntergruppe.Steuersatz.StandardSteuersatz != 0 || artikel.ArtikelUntergruppe.Steuersatz.IstSteuerfrei == true)
        //            {
        //                return artikel.ArtikelUntergruppe.Steuersatz.StandardSteuersatz;
        //            }
        //        }
        //        else if (artikel.ArtikelUntergruppe.ArtikelGruppe != null)
        //        {
        //            if (artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz != null)
        //            {
        //                if (artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.StandardSteuersatz != 0 || artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.IstSteuerfrei == true)
        //                {
        //                    return artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.StandardSteuersatz;
        //                }
        //            }
        //        }
        //    }

        //    //Standard Wert der Steuer, wenn keine andere gefunden wurde
        //    return standardSteuer;
        //}

        //public void ErmittleVerwendeteSteuerTest(Artikel artikel)
        //{
        //    //Überprüfung, ob Artikel, Untergruppe oder Gruppe eine Aktionssteuer hat

        //    if (artikel.Steuersatz != null)
        //    {
        //        if (artikel.Steuersatz.StandardSteuersatz != 0 || artikel.Steuersatz.IstSteuerfrei == true)
        //        {
        //            artikel.verwendeteSteuer = artikel.Steuersatz.StandardSteuersatz;
        //        }
        //    }

        //    if (artikel.ArtikelUntergruppe != null)
        //    {
        //        if (artikel.ArtikelUntergruppe.Steuersatz != null)
        //        {
        //            if (artikel.ArtikelUntergruppe.Steuersatz.StandardSteuersatz != 0 || artikel.ArtikelUntergruppe.Steuersatz.IstSteuerfrei == true)
        //            {
        //                artikel.verwendeteSteuer = artikel.ArtikelUntergruppe.Steuersatz.StandardSteuersatz;
        //            }
        //        }
        //        else if (artikel.ArtikelUntergruppe.ArtikelGruppe != null)
        //        {
        //            if (artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz != null)
        //            {
        //                if (artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.StandardSteuersatz != 0 || artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.IstSteuerfrei == true)
        //                {
        //                    artikel.verwendeteSteuer = artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.StandardSteuersatz;
        //                }
        //            }
        //        }
        //    }

        //    //Standard Wert der Steuer, wenn keine andere gefunden wurde
        //    artikel.verwendeteSteuer = standardSteuer;
        //}


        //public void AktualisiereFürAlleArtikel_VerwendeteSteuer()
        //{
        //    NullOperator null_operator = new NullOperator("GCRecord");
        //    XPCollection<Artikel> artikelListe = new XPCollection<Artikel>(aktiveSession, null_operator);

        //    foreach (Artikel artikel in artikelListe)
        //    {
        //        artikel.verwendeteSteuer = ErmittleVerwendeteSteuer(artikel);
        //    }
        //}

        //public async Task AktualisiereFürAlleArtikel_VerwendeteSteuer_Async()
        //{
        //    NullOperator null_operator = new NullOperator("GCRecord");
        //    XPCollection<Artikel> artikelListe = new XPCollection<Artikel>(aktiveSession, null_operator);
        //    List<Task> tasks = new List<Task>();

        //    //foreach (Artikel artikel in artikelListe)
        //    //{
        //    //    tasks.Add(Task.Run())
        //    //    artikel.verwendeteSteuer = ErmittleVerwendeteSteuer(artikel);
        //    //}
        //}
        #endregion
    }
}
