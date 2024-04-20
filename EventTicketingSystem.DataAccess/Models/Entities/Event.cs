using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    internal class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public int VenueId { get; set; }
        public int EventInfoId { get; set; }
    }
}
