using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace s3cr3tx.Models
{
	public class helpers
	{
		public helpers()
		{
		}
        public static IEnumerable<SelectListItem> PossibleMonths(string defaultValue)
        {
            if (defaultValue == null)
                defaultValue = "";

            return new List<SelectListItem>()
        {
            new SelectListItem() { Text = "1 - Jan", Value = "1", Selected = (defaultValue == "1") },
            new SelectListItem() { Text = "2 - Feb", Value = "2", Selected = (defaultValue == "2") },
            new SelectListItem() { Text = "3 - Mar", Value = "3", Selected = (defaultValue == "3") },
            new SelectListItem() { Text = "4 - Apr", Value = "4", Selected = (defaultValue == "4") },
            new SelectListItem() { Text = "5 - May", Value = "5", Selected = (defaultValue == "5") },
            new SelectListItem() { Text = "6 - Jun", Value = "6", Selected = (defaultValue == "6") },
            new SelectListItem() { Text = "7 - Jul", Value = "7", Selected = (defaultValue == "7") },
            new SelectListItem() { Text = "8 - Aug", Value = "8", Selected = (defaultValue == "8") },
            new SelectListItem() { Text = "9 - Sep", Value = "9", Selected = (defaultValue == "9") },
            new SelectListItem() { Text = "10 - Oct", Value = "10", Selected = (defaultValue == "10") },
            new SelectListItem() { Text = "11 - Nov", Value = "11", Selected = (defaultValue == "11") },
            new SelectListItem() { Text = "12 - Dec", Value = "12", Selected = (defaultValue == "12") },
        };
        }
        public static IEnumerable<SelectListItem> PossibleCarriers(string defaultValue)
        {
            if (defaultValue == null)
                defaultValue = "";

            return new List<SelectListItem>()
        {
            new SelectListItem() { Text = "ATT", Value = "ATT", Selected = (defaultValue == "1") },
            new SelectListItem() { Text = "T-Mobile", Value = "T-Mobile", Selected = (defaultValue == "2") },
            new SelectListItem() { Text = "US Cellular", Value = "USCellular", Selected = (defaultValue == "3") },
            new SelectListItem() { Text = "Verizon", Value = "Verizon", Selected = (defaultValue == "4") },
            new SelectListItem() { Text = "Other", Value = "Other", Selected = (defaultValue == "5") }
            };
        }
    

    }
}

