# TravelSiteModification

Home Pages:
- Jans Tarriela:
- Johnny Tan:

**Third Party API**

  In this project, our third party API was the use of the Ticketmaster API. In our travel website, the user has the option of searching for events via Ticketmaster Affiliate Search, rather than the way the user can search for Flights, Hotels, and Cars. If the user decides to search for Ticketmaster events, the user will be able to search for Ticketmaster events in a specific city. A card gallery/menu will pop up displaying all upcoming Ticketmaster events, with the ability to go to Ticketmaster's website for specific events and book tickets via Ticketmaster's website, instead of booking an event through our Website. The user still has the ability to search and book events via our vacation package regardless, but Ticketmaster is an option via their API.
- Views that use this API:

**Learning Opportunities**

Johnny:
- Views that used this learning opportunity:

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
