using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json;


namespace HouseExchange
{
    public class House
    {
        public int ID { get; set; }
        public string OwnerName { get; set; }
        public string District { get; set; }
        public double Area { get; set; }
        public int Floor { get; set; }
        public string HouseType { get; set; }
        public List<string> Info { get; set; }
        public List<string> Requirements { get; set; }
        public int Requests { get; set; }
        public bool HasBeenInvited { get; set; }

        public House(int id, string ownerName, string district, double area, int floor, string houseType, List<string> info, List<string> requirements)
        {
            ID = id;
            OwnerName = ownerName;
            District = district;
            Area = area;
            Floor = floor;
            HouseType = houseType;
            Info = info;
            Requirements = requirements;
            Requests = -1;
            HasBeenInvited = false;
        }

        public bool IfInvited()
        {
            return Requests != -1;
        }

    }

    class Database
    {
        private string path;

        public Database(string path)
        {
            this.path = path;
        }

        public List<House> GetAll()
        {
            if (!File.Exists(path))
            {
                return new List<House>();
            }

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<House>>(json);
        }

        public void Add(House house)
        {
            List<House> houses = GetAll();
            houses.Add(house);
            SaveAll(houses);
        }

        public void Delete(int id)
        {
            List<House> houses = GetAll();

            houses.RemoveAll(u => u.ID == id);
            SaveAll(houses);
        }

        public void Update(House inp)
        {
            List<House> houses = GetAll();
            int i = 0;

            foreach (House house in houses)
            {
                if (house.ID == inp.ID)
                {
                    houses[i] = house;
                    SaveAll(houses);
                    break;
                }

                i++;
            }
        }

        public bool IsEmpty()
        {
            return !File.Exists(path) || new FileInfo(path).Length == 0;
        }
       
        public void SaveAll(List<House> houses)
        {
            string json = JsonConvert.SerializeObject(houses);
            File.WriteAllText(path, json);
        }
    }

    class Program
    {
        private static Database database;

        static void Main(string[] args)
        {
            database = new Database("database.json");

            while (true)
            {
                Console.WriteLine("*------Вітаємо в головному меню!------*");
                Console.WriteLine("Оберіть один з перелічених варіантів");
                Console.WriteLine("");
                Show.Menu(new string[] { "Обрати iнсуюче житло", "Зареєструвати нове житло", "Вийти з додатку" });

                int choice = Show.Options(3);
                Console.Clear();

                switch (choice)
                {
                    case 1:
                        Access();
                        break;
                    case 2:
                        AddNewHouse(true);
                        break;
                    case 3:
                        Console.WriteLine("До побачення!");
                        return;
                }

                Console.Clear();

            }

        }

        static void Access()
        {
            if (database.IsEmpty())
            {
                Console.WriteLine("Зарестрованого житла нема, додайте нове.");
                Thread.Sleep(2000);
                return;
            }

            Console.WriteLine("*------Вітаємо! Оберіть ваше житло------*");
            List<House> houses = database.GetAll();

            for (int i = 0; i < houses.Count; i++)
            {
                House house = houses[i];
                Console.WriteLine($"{i + 1}. Ім'я: {house.OwnerName}. Район: {house.District}. Тип житла: {house.HouseType}.");
            }

            int choice = Show.Options(houses.Count);
            House house1 = houses[choice - 1];
            House selectedHouse = house1;

            Console.WriteLine($"Вітємо, {selectedHouse.OwnerName}! Ваш район: {selectedHouse.District}\n");
            Menu(selectedHouse);

            Console.Clear();
        }

        static void AddNewHouse(bool Working)
        {
            static string AddOwnerName()
            {
                Console.Write("\nВведіть iм'я власника житла: ");
                string res = Console.ReadLine();

                if (res.Split(" ").Length > 2)
                {
                    Console.WriteLine("Надана інформація некоректна. Введіть тількі ім'я");
                    return AddOwnerName();
                }

                return res;
            }

           static string AddHouseType()
           {
                Console.Write("\nОберіть тип вашого житла. Це дім чи квартира? ");
                string res = Console.ReadLine();
                if (res.ToLower() != "дім" && res.ToLower() != "квартира")
                {
                    Console.WriteLine("Надана інформація некоректна. Вкажіть тип житла: дім чи квартира");
                    return AddHouseType();
                }

                return res;
           }

            Console.WriteLine("*------Створюєм профіль з житлом------*");
            string ownerName = AddOwnerName();
            string houseType = AddHouseType();

            Random random = new Random();
            int id = random.Next(10, 100);

            Console.Write("\nВведіть ваш район: ");
            string district = Console.ReadLine();


            Console.Write("\nВведіть площу: ");
            double area = double.Parse(Console.ReadLine());

            Console.Write("\nВведіть поверх (або кількість поверхів в вашому будинку): ");
            int floor = int.Parse(Console.ReadLine());

            List<string> info = new List<string>();
            Console.WriteLine("\nВведіть переваги вашого житла (через кому): ");
            string[] infoArray = Console.ReadLine().Split(',');
            info.AddRange(infoArray);

            List<string> requirements = new List<string>();
            Console.WriteLine("\nВведіть власні вимоги до житла (через кому): ");
            string[] requirementsArray = Console.ReadLine().Split(',');
            requirements.AddRange(requirementsArray);

            House newHouse = new House(id, ownerName, district, area, floor, houseType, info, requirements);
            database.Add(newHouse);

            Console.WriteLine("\nЖитло додане! Дякуєм.");
            Thread.Sleep(2000);
            Console.Clear();

            if (Working)
            {
               Menu(newHouse);
            }

        }

        static void Menu(House house)
        {
            while (true)
            {
                Console.WriteLine("*------Головне меню------*");

                Show.Menu(new string[] { "Додати житло", "Видалити житло", "Пошук житла", "Переглянути надіслані пропозиції", "Вихiд" });

                int choice = Show.Options(5);
                Console.Clear();


                switch (choice)
                {
                    case 1:
                        AddNewHouse(false);
                        break;
                    case 2:
                        DeleteHouse(house.ID);
                        break;
                    case 3:
                        HousesSearch(house);
                        break;
                    case 4:
                        ViewRequests(house);
                        break;
                    case 5:
                        Console.WriteLine("До побачення!");
                        return;
                }

                Console.Clear();

            }
        }

        static void DeleteHouse(int HouseId)
        {
            Console.WriteLine("*------Видалення житла------*");
            Console.WriteLine("Оберiть житло, яке бажаєте видалити. Для повернення до попередньої сторінки нажміть 0.");

            List<House> houses = database.GetAll();

            for (int i = 0; i < houses.Count; i++)
            {
                House house = houses[i];
                Console.WriteLine($"{i + 1}.Ім'я: {house.OwnerName}. Район: {house.District}. Тип житла: {house.HouseType}.");
            }

            int choice = Show.Options(houses.Count);

            if (choice == 0) { return; }

            House selectedHouse = houses[choice - 1];

            if (selectedHouse.ID == HouseId)
            {
                Console.WriteLine("\nЦе ваше житло, вийдіть з профілю, перш ніж видаляти його.");
                Thread.Sleep(2000);
                return;
            }

            if (selectedHouse.IfInvited())
            {
                House invited = OptionID(selectedHouse.Requests);
                invited.Requests = -1;
                invited.HasBeenInvited = false;
                database.Update(invited);
            }

            database.Delete(selectedHouse.ID);
            Console.WriteLine($"Добре! Житло з району:{selectedHouse.District} видалено.");
        }

        static void HousesSearch(House house)
        { 
            Console.WriteLine("*------Пошук житла------*");

            List<House> houses = database.GetAll();

            if (houses.Count == 0)
            {
                Console.WriteLine("На жаль, зареєстрованого житла поки що немає, тому пошук неможливий");
                Thread.Sleep(2000);
                return;
            }

            Console.WriteLine("Оберiть номер житла для створення запрошення на обмін.\nДля повернення до попередньої сторінки нажміть 0:");

            for (int i = 0; i < houses.Count; i++)
            {
                House h_option = houses[i];
                Console.WriteLine($"\n{i + 1}. Ім'я власника: {h_option.OwnerName}; Район: {h_option.District}; Тип житла: {h_option.HouseType}; Площа: {h_option.Area}; Етаж: {h_option.Floor}.");
            }

            int choice = Show.Options(houses.Count);
            if (choice == 0) return;
            House selectedHouse = houses[choice - 1];

            house.Requests = selectedHouse.ID;
            selectedHouse.Requests = house.ID;
            selectedHouse.HasBeenInvited = true;

            Console.WriteLine($"\nЗапрошення вiдправлено до житла у район: {selectedHouse.District}!");
            database.Update(house);
            database.Update(selectedHouse);
            Thread.Sleep(2000);
        }

        static House OptionID(int ID)
        {
            List<House> all = database.GetAll();
            foreach (House selectedHouse in all)
            {
                if (selectedHouse.ID == ID)
                {
                    return selectedHouse;
                }
            }
            return null;
        }


        static void ViewRequests(House house)
        {
            static void CancelRequest(House house, House houseRequest)
            {
                house.Requests = -1;
                houseRequest.Requests = -1;
                house.HasBeenInvited = false;
                houseRequest.HasBeenInvited = false;

                database.Update(houseRequest);
                database.Update(house);

            }

            Console.WriteLine("*------Запит, який ви надіслали------*");
            House houseRequest = OptionID(house.Requests);

            if (houseRequest == null)
            {
                Console.WriteLine("Ви поки що не надіслали жодного запиту.");
                Thread.Sleep(2000);
                return;
            }
            Console.WriteLine($"\nІм'я власника: {houseRequest.OwnerName}; Район: {houseRequest.District}; Тип житла: {houseRequest.HouseType}; Площа: {houseRequest.Area}; Етаж: {houseRequest.Floor}.\n");
            if (!house.HasBeenInvited)
            {
                Show.Menu(new string[] { "Скасувати", "Вихiд" });
                int choice = Show.Options(2);
                switch (choice)
                {
                    case 1:
                        CancelRequest(house, houseRequest);
                        Console.WriteLine("\nЗапит скасовано");
                        Thread.Sleep(2000);
                        return;
                    case 2:
                        return;
                }
            }
        }
    }

    class Show
    {
        public static void Menu(string[] options)
        {

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }
        }


        public static int Options(int option)
        {
            int choice;
            Console.WriteLine("");

            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > option)
            {
                if (choice == 0) return 0;             
                
                Console.WriteLine("Команда не вірна");
                Console.WriteLine("");
            }
            return choice;
        }

    }

    
}