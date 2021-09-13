using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auftragserfassung_Blazor.Module.Helpers
{
    class ZeitraumHelper2
    {
        public ZeitraumHelper2(Aktion aktion, DateTime aktionStart, DateTime aktionEnde)
        {
            ImportierteAktion = aktion;
            NeueAktionStart = aktionStart;
            NeueAktionEnde = aktionEnde;
        }
        public Aktion ImportierteAktion { get; set; }
        public DateTime NeueAktionStart { get; set; }
        public DateTime NeueAktionEnde { get; set; }


        public bool ÜberprüfeAufSinnvollenAktionszeitraum()
        {
            //liegt das Ende vor dem Anfang?
             if (ImportierteAktion.AktionStart > ImportierteAktion.AktionEnde)
            {
                throw new UserFriendlyException($"Fehler: Das Aktionsende {NeueAktionEnde} liegt vor dem Aktionsanfang {NeueAktionStart}!");
            }

            return true;
        }

        public bool IstAktionAktiv()
        {
            if(DateTime.Today > ImportierteAktion.AktionStart && DateTime.Today < ImportierteAktion.AktionEnde)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void IstNeueAktionZulässig()
        {
            bool keinenFehlerGefunden = ÜberprüfeAufSinnvollenAktionszeitraum();
            if(keinenFehlerGefunden == true)
            {
                int eigenePosition = 0;
                IEnumerable<Aktion> andereAktionenListe = null;


                if (ImportierteAktion.GetType() == typeof(VoruebergehendeSteuer))
                {
                    andereAktionenListe = ((VoruebergehendeSteuer)ImportierteAktion).Steuer.VoruebergehendeSteuerListe.ToList();
                    for (int i = 0; i < andereAktionenListe.Count(); i++)
                    {
                        if (ImportierteAktion.Oid == andereAktionenListe.ElementAt(i).Oid)
                        {
                            eigenePosition = i;
                            break;
                        }
                    }
                }
                else if (ImportierteAktion.GetType() == typeof(Aktionspreis))
                {
                    andereAktionenListe = ((Aktionspreis)ImportierteAktion).AktionsArtikel.AktionspreiseListe.ToList();
                    for (int i = 0; i < andereAktionenListe.Count(); i++)
                    {
                        if (ImportierteAktion.Oid == andereAktionenListe.ElementAt(i).Oid)
                        {
                            eigenePosition = i;
                            break;
                        }
                    }
                }
                else if (ImportierteAktion.GetType() == typeof(Aktionsrabatt))
                {
                    if(((Aktionsrabatt)ImportierteAktion).AktionsArtikel != null)
                    {
                        andereAktionenListe = ((Aktionsrabatt)ImportierteAktion).AktionsArtikel.AktionsrabatteListe.ToList();
                    }
                    else if(((Aktionsrabatt)ImportierteAktion).ArtikelUntergruppe != null)
                    {
                        andereAktionenListe = ((Aktionsrabatt)ImportierteAktion).ArtikelUntergruppe.AktionsrabattListe.ToList();
                    }
                    else if(((Aktionsrabatt)ImportierteAktion).ArtikelGruppe != null)
                    {
                        andereAktionenListe = ((Aktionsrabatt)ImportierteAktion).ArtikelGruppe.AktionsRabattListe.ToList();
                    }

                    for (int i = 0; i < andereAktionenListe.Count(); i++)
                    {
                        if (ImportierteAktion.Oid == andereAktionenListe.ElementAt(i).Oid)
                        {
                            eigenePosition = i;
                            break;
                        }
                    }
                }


                //Überprüfung, ob die Eingabe in einen Aktionszeitraum liegt
                int counter = 0;
                //bool neueAktionIstUnzulässig = false;
                foreach (Aktion schonVorhandeneAktion in andereAktionenListe)
                {
                    if (eigenePosition == counter)
                    {
                        continue;
                    }

                    //liegt Start/Ende in einen Zeitraum?
                    //Start
                    if ((ImportierteAktion.AktionStart >= schonVorhandeneAktion.AktionStart) && (ImportierteAktion.AktionStart <= schonVorhandeneAktion.AktionEnde))
                    {
                        //neueAktionIstUnzulässig = true;
                        throw new UserFriendlyException($"Fehler: Der Aktionsstart liegt in der bereits vorhandenen Aktion vom {NeueAktionStart} bis {NeueAktionEnde}!");
                    }
                    //Ende
                    else if ((ImportierteAktion.AktionEnde >= schonVorhandeneAktion.AktionStart) && (ImportierteAktion.AktionEnde <= schonVorhandeneAktion.AktionEnde))
                    {
                        //neueAktionIstUnzulässig = true;
                        throw new UserFriendlyException($"Fehler: Das Aktionende liegt in der bereits vorhandenen Aktion vom {NeueAktionStart} bis {NeueAktionEnde}!");
                    }
                    //wird ein Aktionszeitraum "umschlossen"?
                    else if ((ImportierteAktion.AktionStart <= schonVorhandeneAktion.AktionStart) && (ImportierteAktion.AktionEnde >= schonVorhandeneAktion.AktionEnde))
                    {
                        //neueAktionIstUnzulässig = true;
                        throw new UserFriendlyException($"Fehler: Es wird die bereits vorhandene Aktion vom {schonVorhandeneAktion.AktionStart} bis {schonVorhandeneAktion.AktionEnde} eingeschlossen!");
                    }

                    counter++;
                }


            //    //Wenn die User Eingaben keinen anderen Aktionszeitraum verletzen
            //    if (neueAktionIstUnzulässig == false)
            //    {
            //        return true;
            //    }
            }
            
            ////Wenn ein Fehler gefunden wurde oder die Aktion unzulässig ist
            //return false;
        }
    }
}

