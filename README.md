# AGL Developer Test - Programming challenge (Azure Function) #

This provides a trial code repository for coding test in AGL.


## Requirements ##

* Data source, a JSON object by sending an HTTP request to [http://agl-developer-test.azurewebsites.net/people.json](http://agl-developer-test.azurewebsites.net/people.json)
* The data source needs to be processed to display result with the rules of:
  * All cats in alphabetical order
  * All cats under their owners' gender
  * Pet owners' genders as headings
* LINQ for grouping and sorting the data source


## Prerequisites ##

* [Visual Studio 2017 (v15.5.2)](https://www.visualstudio.com/)
* [Azure Storage Emulator v5.3](https://docs.microsoft.com/en-au/azure/storage/common/storage-use-emulator)


## Implementation ##

**serverless** architecture &ndash; Azure Functions for

* not to setup an application environment
* to focus on the code itself, which is basically business logic
* to make easy to build

The application consists of number of small libraries that only take care of one responsibility respectively &ndash; settings, models, services, functions and IoC. 
Also each project has its corresponding test projects for unit testing.


## Reproduction ##

The application can be run on either local machine or Azure Functions instance on Azure. 
In order to run this, use Visual Studio or Azure Function Tooling npm package. 
If Visual Studio, follow the steps below:

* Clone the repository to your local machine.
* Open `AGLCodeTest.sln`.
* Build the solution.
* Press F5 key to run the Azure Function instance locally.
* Open a web browser and type `http://localhost:7071/pets` to run the Azure Functions application.
* The required result will be displayed.
* A querystring parameter, `type` can be used to specify the pet type. Currently, the `type` parameter only considers three values &ndash; `Dog`, `Cat` and `Fish`

Valid request sampless

* [http://localhost:7071/pets](http://localhost:7071/pets)
* [http://localhost:7071/pets?type=cat](http://localhost:7071/pets?type=cat)
* [http://localhost:7071/pets?type=dog](http://localhost:7071/pets?type=dog)
* [http://localhost:7071/pets?type=fish](http://localhost:7071/pets?type=fish)

