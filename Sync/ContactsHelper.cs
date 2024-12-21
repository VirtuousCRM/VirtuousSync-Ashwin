using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    internal static class ContactsHelper
    {
        public static List<AbbreviatedContact> FilterContactsByState(List<AbbreviatedContact> contacts, string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                return contacts;
            }

            var stateContacts = new List<AbbreviatedContact>();

            foreach (var contact in contacts)
            {
                var address = contact.Address;

                if (string.IsNullOrEmpty(address)) continue;

                var addressChunks = address.Split(',');

                if (addressChunks.Length > 1)
                {
                    var stateZip = addressChunks[1];
                    var contactState = stateZip.Trim().Split(' ')[0];

                    if (contactState == state)
                    {
                        stateContacts.Add(contact);
                    }
                }
            }

            return stateContacts;
        }
    }
}
