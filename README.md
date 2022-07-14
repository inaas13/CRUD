# CRUD
Databasinlämning
Idag torsdag och fram tills vi ses så är uppgiften att skapa ett program med ett antal metoder som använder sig av ado.net för att köra CRUD mot northwinddatabasen eller Telerikdatabasen.

Pröva gärna att köra både parametriserat och som genererad SQLfråga där eventuella värden läggs in via konkatenering av en stäng.


I allt ni gör så är det stränghantering i C# som kommer vara grunden. Glöm inte att testa era frågor i SSMS 
Programmet skall ha följande metoder som kan ta emot lämpliga parametrar och returvärden. De skall kunna anropas utifrån och behöver därför ta emot sin data via parametrar. Databasuppkopplingen kan skickas som en parameter eller göras inom metoden.
AddCustomer - Lägg till en ny kund
DeleteCustomer - Ta bort en kund baserat på namn eller id. Tänk overloading OBS Denna kan kräva flera steg
UpdateEmployee - Uppdatera existerande anställd med ny adressdata
ShowCountrySales - Visa ordervärdet för ett valfritt land  grupperat på säljare - utskrift på skärm
samt

En sammansatt metod som kan lägga en ny order för en ny kund, ordern måste innehålla minst en vara. – Behöver ingen användarinput

En metod som kan ta bort den kund som ni skapade i metoden ovan. Behöver ingen användarinput  Kundens lagda ordrar behöver inte finnas kvar. Ni får INTE ändra tabellernas schema/struktur. Dvs ingen create, alter, drop

