using Auftragserfassung_Blazor.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auftragserfassung_Blazor.Module.Interfaces
{
    public interface IPosition
    {
        Artikel Artikel { get; set; }
        string Bezeichnung { get; set; }
        double Preis { get; set; }
        double Steuersatz { get; set; }
        double Zeilenrabatt { get; set; }
        int Positionsnummer { get; set; }

    }
}
