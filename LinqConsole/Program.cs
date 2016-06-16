using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.Domain;
using Talent.Domain.TestData;
using UclaExt.Common;
using UclaExt.Common.Extensions;

namespace LinqConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LINQ Lab:");
            DomainObjectStore store = new DomainObjectStore();
            store.People.PrintToConsole();

            Console.WriteLine("\r\nFiltering (Where):");
            IEnumerable<Person> peopleS = store.People
                .Where(e => e.FirstName.StartsWith("s",
                    StringComparison.CurrentCultureIgnoreCase));
            peopleS.PrintToConsole();

            Console.WriteLine("\r\n'S' names over 50 years old:");
            string namePrefix = "s";
            IEnumerable<Person> seniorSPeople = store.People
                .Where(e => e.FirstName.StartsWith(namePrefix,
                    StringComparison.CurrentCultureIgnoreCase))
                .Where(p => p.Age.HasValue && p.Age.Value > 55);
            seniorSPeople.PrintToConsole();


            //Console.WriteLine("\r\n'S' names over 50 years old:");
            //string namePrefix = "s";
            //IEnumerable<Person> seniorSPeople = store.People
            //    .Where(e => e.FirstName.StartsWith(namePrefix,
            //        StringComparison.CurrentCultureIgnoreCase)
            //        && e.Age.HasValue 
            //        && e.Age.Value > 55);
            //seniorSPeople.PrintToConsole();

            Console.WriteLine("\r\nPeople <= 64 inches tall or with no height entered:");
            var maxHeight = 64;
            store.People
                .Where(p => !p.Height.HasValue || p.Height.Value <= maxHeight)
                .PrintToConsole();

            Console.WriteLine("Step 4: Sorting");
            Console.WriteLine("\r\nSort by Age");
            store.People
                .OrderBy(p => p.Age)
                .PrintToConsole();

            // The following should work, but does not 
            // results in an infinite number of calls to the 
            // CompareTo method of Person
            //Console.WriteLine("\r\nSort by Default sort order:");
            //store.People
            //    .OrderBy(p => p, Comparer<Person>.Default)
            //    .PrintToConsole();

            Console.WriteLine("\r\nSort by Age, then by LastFirstName");
            store.People
                .OrderBy(p => p.Age)
                .ThenBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .PrintToConsole();

            Console.WriteLine("\r\nStep 5: Sort by Height "
                + "descending with null heights first, "
                + "then by LastFirstName");
            foreach (var p in
                store.People
                .OrderBy(p => p.Height.HasValue ? 1 : 0)
                .ThenByDescending(p => p.Height)
                .ThenBy(p => p.LastName))
            {
                Console.WriteLine(p.FirstLastName + " - Height: "
                     + (p.Height.HasValue ?
                     p.Height.Value.ToString() : "(null)"));
            }

            Console.WriteLine("\r\nStep 6: Select Method:");
            var ratingCodes = store.MpaaRatings.Select(r => r.Code);
            foreach (string code in ratingCodes)
            {
                Console.WriteLine(code);
            }

            Console.WriteLine("\r\nSelection with custom class for result:");
            var personList = store.People.Select(p =>
                new PersonListItem()
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName
                });
            foreach (PersonListItem p in personList)
            {
                Console.WriteLine(p.ToString());
            }

            Console.WriteLine("\r\nSelection with anonymous class result:");
            var personResult = store.People
                .Select(p =>
                    new
                    {
                        p.Id,
                        p.FirstName,
                        p.LastName
                    });
            foreach (var p in personResult)
            {
                Console.WriteLine(p.ToString());
            }

            Console.WriteLine("\r\nStep 7: Select exercise:");
            var personBirthdates = store.People
                .Select(p =>
                   new { p.LastFirstName, p.DateOfBirth });
            foreach (var pbd in personBirthdates)
            {
                Console.WriteLine(pbd.ToString());
            }

            Console.WriteLine("\r\nStep 8: Combining Fitering, Sorting and Selection:");
            var comboResult = store.People
                .Where(p => p.Height != null)
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .Select(p =>
                    new { p.FirstName, p.LastName, p.Height });
            foreach (var pbd in comboResult)
            {
                Console.WriteLine(
                    String.Format("{0} {1} : {2}",
                    pbd.FirstName, pbd.LastName, pbd.Height.Value)
                    );
            }

            Console.WriteLine("\r\nStep 9: Element Operators:");
            var firstS = store.People
                .Where(p =>
                        p.FirstLastName.ToUpper().Contains("S"))
                        .First();
            Console.WriteLine("First S name: " + firstS.Display());

            var firstZZZ = store.People
                .Where(p =>
                    p.FirstLastName.ToUpper().Contains("ZZZ"))
                    .FirstOrDefault();
            Console.WriteLine("First ZZZ name: " + 
                (firstZZZ == null ? "(None)" : firstZZZ.Display()));

            Console.WriteLine("\r\nStep 10: Grouping:");
            var hairGroups = store.People
                .GroupBy(e => e.HairColorId);
            foreach (var grp in hairGroups)
            {
                Console.WriteLine("Hair Color: "
                    + (store.HairColors.Where(h => h.Id == grp.Key)
                        .Select(h => h.Name).FirstOrDefault())
                    + "\t(" + grp.Count() + ")");
                foreach (var p in grp)
                {
                    Console.WriteLine("\t" + p.ToString());
                }
            }

            Console.WriteLine("\r\nStep 11: Aggregates");
            Console.WriteLine("Latest Birthdate: " + store.People
                .Max(e => e.DateOfBirth));
            Console.WriteLine("Avg Height: " + store.People
                .Average(s => s.Height));
            Console.WriteLine("Count: " + store.People
                .Count());

            Console.WriteLine("Press <Enter> to quit the application");
            Console.ReadLine();
        }

        public class PersonListItem
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public override string ToString()
            {
                return Id.ToString() + ": "
                        + FirstName + " " + LastName;
            }
        }


    }
}
