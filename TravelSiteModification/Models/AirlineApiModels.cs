namespace TravelSiteModification.Models
{
    public class AirlineApiFlight
    {
        private int FlightID;
        private int AirlineID;
        private string AirlineName;
        private string FlightNumber;
        private string FlightClass;

        private string DepartureCity;
        private string DepartureState;
        private string ArrivalCity;
        private string ArrivalState;

        private string DepartureTime;
        private string ArrivalTime;

        private double FlightPrice;
        private int MaxOccupancy;
        private bool AtMaxOccupancy;

        public int flightID
        {
            get { return FlightID; }
            set { FlightID = value; }
        }
        public int airlineID
        {
            get { return AirlineID; }
            set { AirlineID = value; }
        }

        public string airlineName
        {
            get { return AirlineName; }
            set { AirlineName = value; }
        }

        public string flightNumber
        {
            get { return FlightNumber; }
            set { FlightNumber = value; }
        }

        public string flightClass
        {
            get { return FlightClass; }
            set { FlightClass = value; }
        }

        public string departureCity
        {
            get { return DepartureCity; }
            set { DepartureCity = value; }
        }

        public string departureState
        {
            get { return DepartureState; }
            set { DepartureState = value; }
        }

        public string arrivalCity
        {
            get { return ArrivalCity; }
            set { ArrivalCity = value; }
        }

        public string arrivalState
        {
            get { return ArrivalState; }
            set { ArrivalState = value; }
        }

        public string departureTime
        {
            get { return DepartureTime; }
            set { DepartureTime = value; }
        }

        public string arrivalTime
        {
            get { return ArrivalTime; }
            set { ArrivalTime = value; }
        }

        public double flightPrice
        {
            get { return FlightPrice; }
            set { FlightPrice = value; }
        }

        public int maxOccupancy
        {
            get { return MaxOccupancy; }
            set { MaxOccupancy = value; }
        }

        public bool atMaxOccupancy
        {
            get { return AtMaxOccupancy; }
            set { AtMaxOccupancy = value; }
        }
    }

    public class AirlineApiAirline
    {
        private int AirlineID;
        private string AirlineName;
        private string AirlineDescription;
        private string AirlineHeadquarters;
        private string AirlinePhoneNumber;
        private string AirlineEmail;
        private string AirlineLogo;
        private string AirlineWebsite;

        public int airlineID
        {
            get { return AirlineID; }
            set { AirlineID = value; }
        }

        public string airlineName
        {
            get { return AirlineName; }
            set { AirlineName = value; }
        }

        public string airlineDescription
        {
            get { return AirlineDescription; }
            set { AirlineDescription = value; }
        }

        public string airlineHeadquarters
        {
            get { return AirlineHeadquarters; }
            set { AirlineHeadquarters = value; }
        }

        public string airlinePhoneNumber
        {
            get { return AirlinePhoneNumber; }
            set { AirlinePhoneNumber = value; }
        }

        public string airlineEmail
        {
            get { return AirlineEmail; }
            set { AirlineEmail = value; }
        }

        public string airlineLogo
        {
            get { return AirlineLogo; }
            set { AirlineLogo = value; }
        }

        public string airlineWebsite
        {
            get { return AirlineWebsite; }
            set { AirlineWebsite = value; }
        }
    }

    public class AirlineApiUser
    {
        private string FirstName;
        private string LastName;
        private string Email;
        private string Address;
        private string City;
        private string State;
        private string ZipCode;
        private string PhoneNumber;

        public string firstName
        {
            get { return FirstName; }
            set { FirstName = value; }
        }

        public string lastName
        {
            get { return LastName; }
            set { LastName = value; }
        }

        public string email
        {
            get { return Email; }
            set { Email = value; }
        }

        public string address
        {
            get { return Address; }
            set { Address = value; }
        }

        public string city
        {
            get { return City; }
            set { City = value; }
        }

        public string state
        {
            get { return State; }
            set { State = value; }
        }

        public string zipCode
        {
            get { return ZipCode; }
            set { ZipCode = value; }
        }

        public string phoneNumber
        {
            get { return PhoneNumber; }
            set { PhoneNumber = value; }
        }
    }

    public class AirlineApiReserveRequest
    {
        private int AirlineID;
        private AirlineApiFlight Flight;
        private AirlineApiUser Customer;
        private string TravelSiteURL;
        private string TravelSiteAPIToken;

        public int airlineID
        {
            get { return AirlineID; }
            set { AirlineID = value; }
        }

        public AirlineApiFlight flight
        {
            get { return Flight; }
            set { Flight = value; }
        }

        public AirlineApiUser customer
        {
            get { return Customer; }
            set { Customer = value; }
        }

        public string travelSiteURL
        {
            get { return TravelSiteURL; }
            set { TravelSiteURL = value; }
        }

        public string travelSiteAPIToken
        {
            get { return TravelSiteAPIToken; }
            set { TravelSiteAPIToken = value; }
        }
    }

    public class AirlineApiFlightReservation
    {
        private bool Success;
        private int ReservationID;
        private AirlineApiUser Customer;
        private AirlineApiFlight Flight;

        public bool success
        {
            get { return Success; }
            set { Success = value; }
        }

        public int reservationID
        {
            get { return ReservationID; }
            set { ReservationID = value; }
        }

        public AirlineApiUser customer
        {
            get { return Customer; }
            set { Customer = value; }
        }

        public AirlineApiFlight flight
        {
            get { return Flight; }
            set { Flight = value; }
        }
    }
}
