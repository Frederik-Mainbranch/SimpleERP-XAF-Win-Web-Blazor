using Auftragserfassung_Blazor.Module.BusinessObjects;
using Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auftragserfassung_Blazor.Module.Helpers
{
    class LagerHelper4000
    {

        public LagerHelper4000(Session session, Artikel artikel, bool warenEingang)
        {
            Session = session;
            Artikel = artikel;

            if (warenEingang == true)
            {
                foreach (Lager_ArtikeLager_Zugehoerigkeit zugehoerigkeit in artikel.Lager_ArtikeLager_ZugehoerigkeitsListe)
                {
                    if (zugehoerigkeit.StandardLager == true)
                    {
                        Standardlager = zugehoerigkeit.Lager;
                        Standardlager.Save();
                        AnzahlProStellplatz = zugehoerigkeit.AnzahlVonArtikelnAufEinenStellplatz;
                        break;
                    }
                }

                if(Standardlager == null)
                {
                    throw new UserFriendlyException("Es konnte kein Standard Lager ermittelt werden!");
                }

                foreach (Lager_ArtikeLager_Zugehoerigkeit zugehoerigkeit in artikel.Lager_ArtikeLager_ZugehoerigkeitsListe)
                {
                    if (zugehoerigkeit.Lager.Wareneingang == false && zugehoerigkeit.Lager.Warenausgang == false && zugehoerigkeit.StandardLager == false)
                    {
                        AusweichlagerListe.Add(zugehoerigkeit.Lager);
                        //zugehoerigkeit.Lager.Save();
                    }
                }
            }
        }

        //public LagerHelper4000(Session session)
        //{
        //    Session = session;
        //}

        public Session Session { get; set; }
        public Lager Lager_herkunft { get; set; }
        public Lagerplatz Lagerplatz_herkunft { get; set; }
        public Artikel Artikel { get; set; }
        public int AnzahlProStellplatz { get; set; }
        public List<Lagerplatz> GefundeneLagerplaetzeListe_WE = new List<Lagerplatz>();
        public List<Lager> AusweichlagerListe = new List<Lager>();
        public int AnzahlVonLagerbuchungen { get; set; }
        public int AnzahlVonWarenbewegungen { get; set; }
        public bool StandardLagerIstVoll { get; set; }
        public Lager Standardlager { get; set; }
        public string DebugDateiName { get; } = "LagerHelper4000";



        public void UpdateAnzahlStandardlager()
        {
            if(Standardlager == null)
            {
                throw new UserFriendlyException("Es konnte kein Standard Lager ermittelt werden!");
            }
            UpdateAnzahlProStellplatz(Standardlager);
        }


        public bool ErstelleLagerBuchung(Lager lager, Artikel artikel, int mengeZumEinlagern, bool erstelleWarenBewegung, bool erstelleGefundeneLagerplaetzeListe_WE)
        {
            /* 
             * Logik:
             * 1: Ueberpruefung, wie viele Artikel auf einen Stellplatz in den ausgewaehlten Lager passen
             * 2: Ueberpruefung, ob es in den ausgewaelten Lager schon einen Lagerplatz gibt, wo noch Artikel reinpassen
             * 3: wenn keiner gefunden wurde, wird ein leerer Lagerplatz gesucht
             * 4: es wird berechnet, wie viele Artikel noch auf den ausgewaelten Lagerplatz passen
             * 5: auf den ausgewaelten Lagerplatz wird der Artikel mit seiner Anzahl geschrieben
             * optional 6: wenn kein Lagerplatz ermittelt werden konnte, wird eine Fehlermeldung ausgegeben
             */
            UpdateAnzahlProStellplatz(lager);
            while (mengeZumEinlagern != 0)
            {
                Lagerplatz lagerplatz = ErmittleLagerplatz(lager, artikel);
                if (lagerplatz != null)
                {
                    int verfuegbareLagermenge = BestimmeVerfuegbareMenge(lagerplatz, mengeZumEinlagern);
                    mengeZumEinlagern -= verfuegbareLagermenge;

                    if (erstelleWarenBewegung == false)
                    {
                        lagerplatz.Artikel = artikel;
                        lagerplatz.AnzahlDerArtikel += verfuegbareLagermenge;
                    }

                    AnzahlVonLagerbuchungen++;
                    if (erstelleWarenBewegung == true)
                    {
                        lagerplatz.LagerplatzIstReserviert = true;
                        Waren_Bewegung waren_Bewegung = new Waren_Bewegung(Session)
                        {
                            Artikel = Artikel,
                            Anzahl = verfuegbareLagermenge,
                            Lager_Herkunft = Lager_herkunft,
                            Lagerplatz_Herkunft = Lagerplatz_herkunft,
                            Lager_Ziel = lager,
                            Lagerplatz_Ziel = lagerplatz
                        };
                        lagerplatz.ReserviertFuerWarenBewegung = waren_Bewegung;
                        //Lagerplatz_herkunft.LagerplatzIstReserviert = true;
                        //Lagerplatz_herkunft.ReserviertFuerWarenBewegung = waren_Bewegung;
                        //waren_Bewegung.Lagerplatz_Ziel.LagerplatzIstReserviert = true;
                        //waren_Bewegung.Lagerplatz_Ziel.ReserviertFuerWarenBewegung = waren_Bewegung;

                        AnzahlVonWarenbewegungen++;
                    }
                    if (erstelleGefundeneLagerplaetzeListe_WE == true)
                    {
                        GefundeneLagerplaetzeListe_WE.Add(lagerplatz);
                    }
                    //wenn ein Lagerplatz gefunden wurde, aber noch eine Restmenge besteht, die verarbeitet werden muss
                    continue;
                }
                else
                {
                    //wenn kein Lagerplatz gefunden wurde
                    return false;
                }
            }
            //wenn für die gesamte Mege zum einlagern ein Lagerplatz gefunden wurde
            return true;
        }


        public bool ErstelleLagerBuchung_WarenAusgang(Lager lager_ziel, Lager lager_herkunft, Lagerplatz lagerplatz_herkunft, Artikel artikel, int mengeZumEinlagern)
        {
            /* 
             * Logik:
             * 1: Ueberpruefung, wie viele Artikel auf einen Stellplatz in den ausgewaehlten Lager passen
             * 2: Ueberpruefung, ob es in den ausgewaelten Lager schon einen Lagerplatz gibt, wo noch Artikel reinpassen
             * 3: wenn keiner gefunden wurde, wird ein leerer Lagerplatz gesucht
             * 4: es wird berechnet, wie viele Artikel noch auf den ausgewaelten Lagerplatz passen
             * 5: auf den ausgewaelten Lagerplatz wird der Artikel mit seiner Anzahl geschrieben
             * optional 6: wenn kein Lagerplatz ermittelt werden konnte, wird eine Fehlermeldung ausgegeben
             */

            UpdateAnzahlProStellplatz(lager_ziel);
            while (mengeZumEinlagern != 0)
            {
                Lagerplatz lagerplatz = ErmittleLagerplatz(lager_ziel, artikel);
                if (lagerplatz != null)
                {
                    int verfuegbareLagermenge = BestimmeVerfuegbareMenge(lagerplatz, mengeZumEinlagern);
                    mengeZumEinlagern -= verfuegbareLagermenge;
                    lagerplatz.LagerplatzIstReserviert = true;

                    Waren_Bewegung waren_Bewegung = new Waren_Bewegung(Session)
                    {
                        Artikel = Artikel,
                        Anzahl = verfuegbareLagermenge,
                        Lager_Herkunft = lager_herkunft,
                        Lagerplatz_Herkunft = lagerplatz_herkunft,
                        Lager_Ziel = lager_ziel,
                        Lagerplatz_Ziel = lagerplatz
                    };
                    lagerplatz.ReserviertFuerWarenBewegung = waren_Bewegung;
                    //wenn ein Lagerplatz gefunden wurde, aber noch eine Restmenge besteht, die verarbeitet werden muss
                    continue;
                }
                else
                {
                    //wenn kein Lagerplatz gefunden wurde
                    return false;
                }
            }
            //wenn für die gesamte Mege zum einlagern ein Lagerplatz gefunden wurde
            return true;
        }


        private Lagerplatz ErmittleLagerplatz(Lager lager, Artikel artikel)
        {
            //Kontrolle auf angefangene Lagerplaetze
            CriteriaOperator criteria_angefangenePlaetze = CriteriaOperator.And(new BinaryOperator("Lager", lager), new BinaryOperator("Artikel", artikel),
                CriteriaOperator.Parse($"AnzahlDerArtikel < {AnzahlProStellplatz}"), new BinaryOperator("LagerplatzIstGesperrt", false), new BinaryOperator("LagerplatzIstReserviert", false));
            Lagerplatz lagerplatz = Session.FindObject<Lagerplatz>(PersistentCriteriaEvaluationBehavior.InTransaction, criteria_angefangenePlaetze);


            if (lagerplatz != null)
            {
                return lagerplatz;
            }
            //Kontrolle auf leere Lagerplaetze
            else if (lagerplatz == null)
            {
                CriteriaOperator criteria_leerePlaetze = CriteriaOperator.And(new BinaryOperator("Lager", lager), new NullOperator("Artikel"), new BinaryOperator("LagerplatzIstGesperrt", false),
                    new BinaryOperator("LagerplatzIstReserviert", false));
                lagerplatz = Session.FindObject<Lagerplatz>(PersistentCriteriaEvaluationBehavior.InTransaction, criteria_leerePlaetze);


                if (lagerplatz != null)
                {
                    return lagerplatz;
                }
            }
            return null;
        }

        private int BestimmeVerfuegbareMenge(Lagerplatz lagerplatz, int verbleibendeMenge)
        {
            /*
             * Logik: ermittelt fuer die Methode "ErmittleLagerplatz" wie viele Artikel noch auf den Lagerplatz passen
             */
            if(AnzahlProStellplatz == 0)
            {
                throw new UserFriendlyException("Fehler beim ermitteln des Lagerplatzes: Die Anzahl pro Stellplatz war 0!");
            }

            int dummy = AnzahlProStellplatz - lagerplatz.AnzahlDerArtikel;
            int verfuegbareLagermenge;
            if (dummy < 0)
            {
                verfuegbareLagermenge = AnzahlProStellplatz - lagerplatz.AnzahlDerArtikel + dummy;
            }
            else
            {
                verfuegbareLagermenge = AnzahlProStellplatz - lagerplatz.AnzahlDerArtikel;
            }

            if (verbleibendeMenge > verfuegbareLagermenge)
            {
                return verfuegbareLagermenge;
            }
            else
            {
                return verbleibendeMenge;
            }
        }

        //private int BestimmeAnzahlProStellplatz_Standard(Artikel artikel)
        //{
        //    int anzahlProStellplatz = 0;
        //    foreach (Lager_ArtikeLager_Zugehoerigkeit zugehoerigkeit in artikel.Lager_ArtikeLager_ZugehoerigkeitsListe)
        //    {
        //        if (zugehoerigkeit.StandardLager == true)
        //        {
        //            anzahlProStellplatz = zugehoerigkeit.AnzahlVonArtikelnAufEinenStellplatz;
        //            break;
        //        }
        //    }
        //    if (anzahlProStellplatz == 0)
        //    {
        //        throw new UserFriendlyException("Dieser Artikel ist nicht für diesen Wareneingang vorgesehen!");
        //    }

        //    return anzahlProStellplatz;
        //}

        public void UpdateAnzahlProStellplatz(Lager lager)
        {
            foreach (Lager_ArtikeLager_Zugehoerigkeit zugehoerigkeit in Artikel.Lager_ArtikeLager_ZugehoerigkeitsListe)
            {
                if (zugehoerigkeit.Lager == lager)
                {
                    AnzahlProStellplatz = zugehoerigkeit.AnzahlVonArtikelnAufEinenStellplatz;
                    lager.Save();
                    break;
                }
            }
        }

        public bool Erstelle_Waren_Bewegung_WarenEingangZuLager(Lager lager_herkunft, Lagerplatz lagerplatz_herkunft, int anzahl)
        {
            Lager_herkunft = lager_herkunft;
            Lagerplatz_herkunft = lagerplatz_herkunft;
            int restMengeZumEinlagern = anzahl;
            int counter_volleAusweichlager = 0;
            int menge;


            while (restMengeZumEinlagern != 0)
            {
                if (restMengeZumEinlagern > AnzahlProStellplatz)
                {
                    menge = AnzahlProStellplatz;
                }
                else
                {
                    menge = restMengeZumEinlagern;
                }

                if (StandardLagerIstVoll == false)
                {
                    // Hier ist der Fehler, es muss die Restmenge benutzt werden
                    if (ErstelleLagerBuchung(Standardlager, Artikel, menge, true, false) == true)
                    {
                        restMengeZumEinlagern -= menge;
                        continue;
                    }
                    else
                    {
                        StandardLagerIstVoll = true;
                        UpdateAnzahlProStellplatz(AusweichlagerListe[counter_volleAusweichlager]);
                    }
                }

                bool erfolgreich = false;
                foreach (Lager ausweichlager in AusweichlagerListe)
                {
                    if (counter_volleAusweichlager > AusweichlagerListe.IndexOf(ausweichlager))
                    {
                        continue;
                    }

                    if (ErstelleLagerBuchung(ausweichlager, Artikel, menge, true, false) == true)
                    {
                        erfolgreich = true;
                        restMengeZumEinlagern -= menge;
                        break;
                    }
                    else
                    {
                        counter_volleAusweichlager++;
                        UpdateAnzahlProStellplatz(AusweichlagerListe[counter_volleAusweichlager]);
                    }
                }
                if (erfolgreich == true)
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        public void ReserviereArtikel_ImWareneingang(int anzahlZuReservieren, List<Lagerplatz> lagerplaetzeListe)
        {
            int anzahl_noch_zu_reservieren = anzahlZuReservieren;
            BinaryOperator istWarenausgangslager = new BinaryOperator("Warenausgang", true);
            Lager warenausgangsLager = Session.FindObject<Lager>(PersistentCriteriaEvaluationBehavior.InTransaction, istWarenausgangslager);

            if (warenausgangsLager != null)
            {
                UpdateAnzahlProStellplatz(warenausgangsLager);

                foreach (Lagerplatz lagerplatz in lagerplaetzeListe)
                {

                    int lagerplatz_anzahl_verfuegbare_artikeln = lagerplatz.AnzahlDerArtikel - lagerplatz.Anzahl_Reserviert;

                    if (lagerplatz_anzahl_verfuegbare_artikeln > 0) //Wenn Artikeln zum reservieren vorhanden sind
                    {
                        if (anzahl_noch_zu_reservieren > lagerplatz_anzahl_verfuegbare_artikeln)
                        {
                            lagerplatz.Anzahl_Reserviert += lagerplatz_anzahl_verfuegbare_artikeln;
                            anzahl_noch_zu_reservieren -= lagerplatz_anzahl_verfuegbare_artikeln;
                        }
                        else //anzahl_noch_zu_reservieren <= lagerplatz_anzahl_an_nicht_reservierten_artikeln
                        {
                            lagerplatz.Anzahl_Reserviert += anzahl_noch_zu_reservieren;
                            anzahl_noch_zu_reservieren = 0;
                        }

                        if (ErstelleLagerBuchung_WarenAusgang(warenausgangsLager, lagerplatz.Lager, lagerplatz, Artikel, lagerplatz_anzahl_verfuegbare_artikeln) == false)
                        {
                            throw new UserFriendlyException("Es konnten keine freien Lagerplätze ermittelt werden!");
                        }

                        if (anzahl_noch_zu_reservieren == 0)
                        {
                            break;
                        }
                    }
                }

                //Wenn zu wenige Artikel verfügbar sind
                Artikel.BerechneAnzahlAnVerfuegbarenArtikeln();
            }
        }

        public void ReserviereArtikel_Im_Lager(int anzahlZuReservieren)
        {
            int anzahl_noch_zu_reservieren = anzahlZuReservieren;
            BinaryOperator istWarenausgangslager = new BinaryOperator("Warenausgang", true);
            Lager warenausgangsLager = Session.FindObject<Lager>(PersistentCriteriaEvaluationBehavior.InTransaction, istWarenausgangslager);


            if (warenausgangsLager != null)
            {
                UpdateAnzahlProStellplatz(warenausgangsLager);

                //GefundeneLagerplaetzeListe_WE
                foreach (Lagerplatz lagerplatz in Artikel.LagerplatzListe)
                {
                    if (lagerplatz.Lager.Wareneingang == true || lagerplatz.Lager.Warenausgang == true)
                    {
                        continue;
                    }

                    int lagerplatz_anzahl_verfuegbare_artikeln = lagerplatz.AnzahlDerArtikel - lagerplatz.Anzahl_Reserviert;

                    if (lagerplatz_anzahl_verfuegbare_artikeln > 0) //Wenn Artikeln zum reservieren vorhanden sind
                    {
                        if (anzahl_noch_zu_reservieren > lagerplatz_anzahl_verfuegbare_artikeln)
                        {
                            lagerplatz.Anzahl_Reserviert += lagerplatz_anzahl_verfuegbare_artikeln;
                            anzahl_noch_zu_reservieren -= lagerplatz_anzahl_verfuegbare_artikeln;
                        }
                        else //anzahl_noch_zu_reservieren <= lagerplatz_anzahl_an_nicht_reservierten_artikeln
                        {
                            lagerplatz.Anzahl_Reserviert += anzahl_noch_zu_reservieren;
                            anzahl_noch_zu_reservieren = 0;
                        }


                        if (ErstelleLagerBuchung_WarenAusgang(warenausgangsLager, lagerplatz.Lager, lagerplatz, Artikel, lagerplatz_anzahl_verfuegbare_artikeln) == false)
                        {
                            throw new UserFriendlyException("Es konnten keine freien Lagerplätze ermittelt werden!");
                        }

                        if (anzahl_noch_zu_reservieren == 0)
                        {
                            break;
                        }
                    }
                }

                //Wenn zu wenige Artikel verfügbar sind
                Artikel.BerechneAnzahlAnVerfuegbarenArtikeln();
            }
        }


        #region alt



        //    if(standardLagerGefunden == true)
        //    {
        //        if (ErmittleLagerplatz(anzahlProStellplatz, lager_standard, artikel, mengeZumEinlagern) != null)
        //        {
        //            ErstelleLagerBuchung(lager_standard)
        //        }
        //    }

        //    bool lagerplatzGefunden = false;

        //    if(ErmittleLagasderplatz(lager_standard, artikel, mengeZumEinlagern, false, true) == false)
        //    {
        //        if (standardLagerGefunden == false)
        //        {
        //            foreach (Lager_ArtikeLager_Zugehoerigkeit zugehoerigkeit in artikel.Lager_ArtikeLager_ZugehoerigkeitsListe)
        //            {
        //                if (zugehoerigkeit.StandardLager == false)
        //                {
        //                    if(ErmittleasdLagerplatz(zugehoerigkeit.Lager, artikel, mengeZumEinlagern, false, true) == true)
        //                    {
        //                        lagerplatzGefunden = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        lagerplatzGefunden = true;
        //    }

        //    if(lagerplatzGefunden == true)
        //    {
        //        foreach (Lagerplatz item in collection)
        //        {

        //        }
        //        Waren_Bewegung waren_Bewegung = new Waren_Bewegung(Session)
        //        {
        //            Artikel = lagerplatz_herkunft.Artikel,
        //            Anzahl = lagerplatz_herkunft.AnzahlDerArtikel,
        //            Lager_Herkunft = lager_herkunft,
        //            Lagerplatz_Herkunft = lagerplatz_herkunft,
        //            Lager_Ziel = lager_standard
        //        };
        //    }
        //}
        #endregion

    }
}
