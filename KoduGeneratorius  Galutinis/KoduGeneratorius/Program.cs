
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace KoduGeneratorius
{
    // Asmens duomenys
    public class Person
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int BirthYear { get; set; }
        public int BirthMonth { get; set;}
        public int BirthDay { get; set;}
    }
    // Asmens duomenys su asmens kodu
    public class PersonWithPersonalCode : Person
    {
        public string PersonalCode { get; set; }
    }
    // Asmens duomenys su asmens kodu ir jo patikrinimu
    public class PersonWithPersonalCodeValidated : PersonWithPersonalCode
    {
        public bool IsValid { get; set; }
    }

    //Pagrindine programos klase
    internal class Program
    {
        const string AsmenysSuAsmensKodaisFailas = "C:\\Users\\tomas\\OneDrive\\Desktop\\C#\\asmensKoduGeneratorius\\KoduGeneratorius  Galutinis\\KoduGeneratorius\\Reikia tikrinti asmens kodus.json";
        const string AsmenuBeAsmensKoduFailas = "C:\\Users\\tomas\\OneDrive\\Desktop\\C#\\asmensKoduGeneratorius\\KoduGeneratorius  Galutinis\\KoduGeneratorius\\Reikia sukurti asmens kodus.json";
        const string SugeneruotuRezultatuFailas = "C:\\Users\\tomas\\OneDrive\\Desktop\\C#\\asmensKoduGeneratorius\\KoduGeneratorius  Galutinis\\KoduGeneratorius\\Sugeneruoti asmens kodai.json";
        const string PatikrintuAsmensKoduFailas = "C:\\Users\\tomas\\OneDrive\\Desktop\\C#\\asmensKoduGeneratorius\\KoduGeneratorius  Galutinis\\KoduGeneratorius\\Patikrinti asmens kodai.json";
        
        //Paleidziamoji programos funkcija
        static void Main(string[] args)
        {

            // Nusiskaitem savo asmenis kuriems reikia sugeneruoti koda
            string json = File.ReadAllText(AsmenuBeAsmensKoduFailas);
            List<Person> persons = JsonConvert.DeserializeObject<List<Person>>(json);
            List<PersonWithPersonalCode> personsWithCodes = GeneratePersonalsCodes(persons);
            // Rasom duomenis i faila
            File.WriteAllText(SugeneruotuRezultatuFailas, JsonConvert.SerializeObject(personsWithCodes, Formatting.Indented) );
            
            // nusiskaitem faila
            json = File.ReadAllText(AsmenysSuAsmensKodaisFailas);
            List<PersonWithPersonalCode> personsWithCodes1 = JsonConvert.DeserializeObject<List<PersonWithPersonalCode>>(json);
            List<PersonWithPersonalCodeValidated> validated = ValidatePersonalCodes(personsWithCodes1);

            File.WriteAllText(PatikrintuAsmensKoduFailas, JsonConvert.SerializeObject(validated, Formatting.Indented));
        }

        //Tikrinam asmens kodu sarasa (List)
        public static List<PersonWithPersonalCodeValidated> ValidatePersonalCodes(List<PersonWithPersonalCode> personsWithCodes)
        {
            List<PersonWithPersonalCodeValidated> validated = new List<PersonWithPersonalCodeValidated>();
            foreach (PersonWithPersonalCode personWithCode in personsWithCodes)
            {
                PersonWithPersonalCodeValidated personWithPersonalCodeValidated = new PersonWithPersonalCodeValidated();
                personWithPersonalCodeValidated.FirstName = personWithCode.FirstName;
                personWithPersonalCodeValidated.LastName = personWithCode.LastName;
                personWithPersonalCodeValidated.Gender = personWithCode.Gender;
                personWithPersonalCodeValidated.BirthYear = personWithCode.BirthYear;
                personWithPersonalCodeValidated.BirthMonth = personWithCode.BirthMonth;
                personWithPersonalCodeValidated.BirthDay = personWithCode.BirthDay;
                personWithPersonalCodeValidated.PersonalCode = personWithCode.PersonalCode;
                personWithPersonalCodeValidated.IsValid = IsValid(personWithCode);

                validated.Add(personWithPersonalCodeValidated);
            }
            return validated;
        }

        // Asmenims sugeneruojame asmens kodus
        public static List<PersonWithPersonalCode> GeneratePersonalsCodes(List<Person> persons)
        {
            List<PersonWithPersonalCode> personWithPersonalCodes= new List<PersonWithPersonalCode>();
            foreach (Person person in persons)
            {
                //sukuriam nauja objekta
                PersonWithPersonalCode personWithPersonalCode = new PersonWithPersonalCode();
                personWithPersonalCode.FirstName = person.FirstName;
                personWithPersonalCode.LastName = person.LastName;
                personWithPersonalCode.Gender = person.Gender;
                personWithPersonalCode.BirthYear = person.BirthYear;
                personWithPersonalCode.BirthMonth = person.BirthMonth;
                personWithPersonalCode.BirthDay = person.BirthDay;
                personWithPersonalCode.PersonalCode = GeneratePersonalCode(person);

                personWithPersonalCodes.Add(personWithPersonalCode);
            }

            return personWithPersonalCodes;
        }

        //Asmens kodo generavimas
        public static string GeneratePersonalCode(Person person)//paduodam Person objekta ir grazinam sugeneruota asmens koda string formatu
        {   
            //skaiciuojam pirma asmens kodo skaiciu lyties
            int genderDigit = 0;
            if (person.BirthYear >= 1800 && person.BirthYear <= 1900)
            {
                genderDigit = (person.Gender == "V") ? 1 : 2;
            }
            else if (person.BirthYear >= 1901 && person.BirthYear <= 2000)
            {
                genderDigit = (person.Gender == "V") ? 3 : 4;
            }
            else if (person.BirthYear >= 2001 && person.BirthYear <= 2100)
            {
                genderDigit = (person.Gender == "V") ? 5 : 6;
            }
            // 1 sk pagal lyti 
            int firstDigit = genderDigit;
            // 2 sk ir 3sk pagal metus
            int secondAndThirdDigits = person.BirthYear % 100;
            // 4sk ir 5 sk pagal menesi
            int fourthAndFifthDigits = person.BirthMonth;
            // 6sk ir 7sk pagal diena
            string sixthAndSeventhDigits = $"{person.BirthDay:D2}";//D2 - darasom 0 jei turim vienzenkli skaiciu

            // trys atsitiktiniai skaiciai
            Random random = new Random();
            int randomDigits = random.Next(1000);
            string randomDigitsString = $"{randomDigits:D3}";
            //pirmi 10 asmens kodo skaiciu
            string partialPersonalCode = $"{firstDigit}{secondAndThirdDigits:D2}{fourthAndFifthDigits:D2}{sixthAndSeventhDigits}{randomDigitsString}";
            //skaiciuojam paskutini kontrolini skaiciu
            string controlDigit = GeneratePersonalCodeControlNumber(partialPersonalCode);

            return partialPersonalCode + controlDigit;
        }

        //Tikrina ar asmuo turi tinkama asmens koda
        public static bool IsValid(PersonWithPersonalCode person)
        { 
            //Ar yra uzpildyta reksme           Ar sudaro tik skaiciai                  Ar ilgis yra 11 simboliu
            if (person.PersonalCode == null || !IsDigitsOnly(person.PersonalCode) || person.PersonalCode.Length != 11)
            {
                return false;
            }
            else
            {  
                //Kvieciam funkcija kuri sugeneruoja asmens koda
                string generatedPersonalCode = GeneratePersonalCode(person);

                //Palyginam turima ir sugeneruota asmens kodus
                return ComparePersonalCodes(person.PersonalCode, generatedPersonalCode);
                
            }
        }

        //Tikrina ar string yra sudarytas tik is skaiciu
        private static bool IsDigitsOnly(string value)
        {
            return Regex.IsMatch(value, "^[0-9]*$");
        }

        //Palygina du asmens kodus ar lygus vienas kitam
        private static bool ComparePersonalCodes(string personalCode1, string personalCode2)
        {
            personalCode1 = personalCode1.Remove(7, 3);//ismetam 3 netikrinamus skaicius
            personalCode2 = personalCode2.Remove(7, 3);

            return personalCode1.Equals(personalCode2);
        }

        //Generuojam asmens kodo paskutini kontrolini skaiciu
        private static string GeneratePersonalCodeControlNumber(string personalCode)
        {
            //Pasiverciam string i int lista
            List<int> personalCodeDigits = personalCode.Select(c => int.Parse(c.ToString())).ToList();
            List<int> personalCodePrefix = personalCodeDigits.Take(10).ToList();
            int personalCodeSuffix = personalCodeDigits.Last();

            int controlNumber = CalculatePersonalCodeControlNumber(personalCodePrefix, factor: 1);//S = A*1 + B*2 + C*3 + D*4 + E*5 + F*6 + G*7 + H*8 + I*9 + J*1

            //Jei liekana lygi 10, tuomet skaičiuojama nauja suma su tokiais svertiniais koeficientais
            if (controlNumber == 10)
            {
                controlNumber = CalculatePersonalCodeControlNumber(personalCodePrefix, factor: 3);//S = A*3 + B*4 + C*5 + D*6 + E*7 + F*8 + G*9 + H*1 + I*2 + J*3
            }

            //Jei controlNumber yra 10 grazinam 0,kitu atveju grazinam controlNumber
            //Jei liekana nelygi 10, ji yra asmens kodo kontrolinis skaičius K. Jei vėl liekana yra 10, kontrolinis skaičius K yra 0.
            return controlNumber == 10 ? "0" : controlNumber.ToString();
        }

        //Skaiciuojam paskutini kontrolini skaiciu
        private static int CalculatePersonalCodeControlNumber(List<int> prefix, int factor)
        {
            var sum = 0;//Skaitmenu ir koeficiantu sandaugu suma
            for (int index = 0; index < prefix.Count; index++)//Sukam cikla per kiekviena skaitmeni
            {
                sum += prefix[index] * factor;//Prie sumos pridedam skaiciaus ir factoriaus daugyba

                factor++;//padidinam vienu skaicium factoriu

                if (factor == 10)//Jei factorius yra 10
                {
                    factor = 1;//Priskiriam factoriui reiksme 1
                }
            }
            return sum % 11;//Grazinam sumos dalybos is 11 likuti
        }
    }
}