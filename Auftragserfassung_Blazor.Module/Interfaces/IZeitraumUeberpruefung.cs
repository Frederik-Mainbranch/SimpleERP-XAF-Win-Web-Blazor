using Auftragserfassung_Blazor.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auftragserfassung_Blazor.Module.Interfaces
{
    public interface IZeitraumUeberpruefung
    {
        DateTime BenutzerEingabeAktionsstart { get; set; }
        DateTime BenutzerEingabeAktionsende { get; set; }
        DateTime AktionStart { get; set; }
        DateTime AktionEnde { get; set; }
         
        Steuer Steuer { get; set; }
        Artikel AktionsArtikel { get; set; }

    }
}
