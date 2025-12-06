# TravelSiteModification

Home Pages:
- Jans Tarriela: https://cis-iis2.temple.edu/Fall2025/CIS3342_tup84792/
- Johnny Tan: https://cis-iis2.temple.edu/Fall2025/CIS3342_tul38243/

**Third Party API**

  In this project, our third party API was the use of the Ticketmaster API. In our travel website, the user has the option of searching for events via Ticketmaster Affiliate Search, rather than the way the user can search for Flights, Hotels, and Cars. If the user decides to search for Ticketmaster events, the user will be able to search for Ticketmaster events in a specific city. A card gallery/menu will pop up displaying all upcoming Ticketmaster events, with the ability to go to Ticketmaster's website for specific events and book tickets via Ticketmaster's website, instead of booking an event through our Website. The user still has the ability to search and book events via our vacation package regardless, but Ticketmaster is an option via their API.
- Views that use this API: TravelSiteModification/Views/Ticketmaster/Index.cshtml
- Side note when it comes to Events and possibly Cars API: For both to work, you may need to right click on solution,, go to Configure Startup Projects, and run the main solution AND both APIs at the same time. You definitely will need to do this at least for the Events API, it must start up and run at the same time as the main web application

**Learning Opportunities**

Johnny: This learning opportunity involves the implementation of SignalR which is a Microsoft.AspNetCore library used in Visual Studio. It provides real-time notifications sent to the specific users' profilew whenever they book a Hotel, Flight, Car, and/or Event/Attraction. It uses a configuration called IHubContext which triggers real-time events. JavaScript is also included in this fully server-side so that the program will listen to the triggers and then send a notification to their account asynchronously when the user presses the book button on the client-side.
- View that used this learning opportunity: TravelSiteModification/Views/Shared/_Layout.cshtml

Jans: This learning opportunity will include the implementation of a pie chart that is generated dynamically. This pie chart would have four categories (Hotels, Flights, Cars, and Events) and indicate how much money the user has spent on each category/service, while updating as the user makes more bookings for each category. This functions as a visual indicator to the user about what their spending habits are on this site on a specific level by category, and also the overall amount that has been spent. This is also entirely implemented server-side, using the ScottPlot application to generate the chart.
- View that used this learning opportunity: TravelSiteModification/Views/Trips/Index.cshtml

**Security**

  We have included security in two parts of this application. First, when the user registers a new account, they must answer 3 security questions, and their answers will be saved in the application. When the user wants to reset their password, they will not only have to answer a randomly selected security question they provided an answer to upon registration, but the web site will send a verification email to them with a button they will have to click in order to successfully reset their password.

**Database**

Name of Database: FA25_3342_tul38243

List of Table names used with this project:

[dbo].[Agencies]

[dbo].[CarAgency]

[dbo].[CarBooking]

[dbo].[EventAttraction]

[dbo].[EventBooking]

[dbo].[Flight]

[dbo].[FlightBooking]

[dbo].[Hotel]

[dbo].[HotelBooking]

[dbo].[HotelRoom]

[dbo].[PackageCars]

[dbo].[PackageEvents]

[dbo].[PackageFlights]

[dbo].[PackageHotels]

[dbo].[PaymentMethod]

[dbo].[RentalCar]

[dbo].[Reservation]

[dbo].[TP_TravelSites]

[dbo].[TravelSiteAuth]

[dbo].[Users]

[dbo].[VacationPackage]

[dbo].[TP_SecurityQuestions]

[dbo].[TP_UserSecurityQuestions]

[dbo].[TP_EventBookingSeat]

[dbo].TP_EventSeat]

Stored Procedures

AddCarBooking
AddEventBooking
AddFlightBooking
AddHotelBooking
AddUser
APIFindCarsByAgency
APIReserveCar
BookFlight
CheckCredentials
CheckUserEmail
CheckUserPassword
GetAllEventsByCity
GetAllFlightsByDestination
GetAllHotelsByCity
GetAllRentalCarsByCity
GetAvailableFlights
GetAvailableRoomsByID
GetBookingDetailsByFlight
GetBookingDetailsByHotelAndRoom
GetCarAgencies
GetCarAgencyByID
GetCarAndAgencyDetails
GetCarByID
GetCarPricesByIDs
GetCarsByPickupAndDropoff
GetEventByID
GetEventDetailsByID
GetEventPricesByIDs
GetEventsByLocation
GetFlightByID
GetFlightPricesByIDs
GetFlightsByDestination
GetFlightsByRoute
GetHotelPricesByIDs
GetHotelsByCity
GetHotelsByID
GetHotelsByIDRefined
GetHotelsByCityRefined
GetOtherAvailableCarsByAgency
GetOtherAvailableCarsByAgencyID
GetRentalCarsByCity
GetRoomByID
GetUserCarBookings
GetUserEventBookings
GetUserFlightBookings
GetUserHotelBookings
GetVacationPackagesByUserID
InsertPackageCar
InsertPackageEvent
InsertPackageFlight
InsertPackageHotel
InsertVacationPackage
spInsertPackageCar
spInsertPackageEvent
 spInsertPackageFlight
TP_spCreateToken
UpdateCarAvailability
UpdateUserProfile
TP_spEventsReserve
TP_spEvents_GetActivities
TP_GetSecurityQuestions
TP_AddUserSecurityQuestions
TP_GetUserSecurityQuestions
TP_UpdateUserPassword
TP_GetVacationPackage
TP_GetPackageFlightDetails
TP_GetPackageEventDetails
TP_ConfirmVacationPackage
TP_GetPackageCarDetails
TP_GetPackageHotelDetails
TP_GetUserSpendingSummary
TP_spEvents_GetSeatsForEvent
TP_GetSeatsForEvent

