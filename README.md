##HotDeskBooking made for Software Minds Internship recruitation

## Description 

Hot desk booking system is a system which should designed to automate the reservation of desk in office through an easy online booking system. 

## Requirements 

Administration: 

- Manage locations (add/remove, can't remove if desk exists in location) 

- Manage desk in locations (add/remove if no reservation/make unavailable) 

Employees 

- Determine which desks are available to book or unavailable. 

- Filter desks based on location 

- Book a desk for the day. 

- Allow reserving a desk for multiple days but now more than a week. 

- Allow to change desk, but not later than the 24h before reservation. 

- Administrators can see who reserves a desk in location, where Employees can see only that specific desk is unavailable.

## How to run

- Project is Dockerized for ease of use. To run the project you just need to use docker command "docker-compose up" or visual studio's built in docker support.
- Project has User accounts(Employee: login="employee", password="employee" Admin: login="admin", password="admin"), Locations and Desks already seeded in db. 

## Technologies

- ASP.NET Core
- EntityFrameworkCore
- JWT Token
- Swagger
- MSSQL
- XUnit
- Moq
- Docker
  
