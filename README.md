# DKExperiments
### This is a pet-project just for some tests, experiments and learning new technilogies (e.g. Docker and Kubernetes, Unit tests frameworks).
### But I'll try to keep it as clean as possible, with descriptions and instructions (because I love clear instructions and explanations).
#### Project contains logic of an aggregator of the BTC-USD exchange rates.

## Description
Aggregator provides users with average BTC (_and in the future not only BTC_) prices aggregated from multiple sources.
It has two endpoints:
	- /prices/load-single
	- /prices/load-range

**load-single** tries to load data from the local database and if it's not there yet - makes requests to specified external APIs to gather price information, then calculates average price and saves it into the local db so it will be available immediately next time.

**load-range** loads already saved records from teh clocal database in the date-time range specified by the user. It doesn't make any requests to external APIs so the response time must be minimal.

---

## Starting the project
There are two ways to start the project.

### 1) Database in Docker and API project from VisualStudio (or other IDE)
To start database navigate to /Database folder of the project in a PowerShell (_or something else if you are not on Windows_) and run the following command: **docker-compose -f database.yaml up**.
Inside the same folder you can find a second compose file named **test-database.yaml**. It is needed to run a database for unit tests in Docker. To start it run the following command: **docker-compose -f test-database.yaml up**. After that you can run unit tests from a project.

To start the project just open it in your IDE and run the **DKExperiments.API** project.
In this case Development settings will be used. You can find them in **appsettings.Development.json** file.

After that the website will be available here: [https://localhost:7155](https://localhost:7155).
To open swagger please use the following link: [https://localhost:7155/swagger/index.html](https://localhost:7155/swagger/index.html).

### 2) Database and API in Docker
To start everything at once please first navigate to the project root folder. There you'll find **docker-compose.yml** file and **Dockerfile**.
To run it please execure the following command in PowerShell (_or something else if you are not on Windows_): **docker compose up --build**.
It will run everything you need in Docker and after that you'll be able to open the website with swagger by the following link: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html).

---

## Services settings and creating more services and aggregate functions

### Settings
In the appsettings.json you can find ome settings for prices parsing services.
You can easily change them later on or add more if needed.

### Creating more parsing services
To create a new service just create a new class implementing **IPricesParcer** interface and write all parsing logic you need.
Then you can activate it for the actual parsing by adding it as a KeyedScoped service (see **Dependency/ServiceCollectionExtensions.cs** file in a project **DKExperiments.Core**) with a key **IPricesParcer**. It will be automatically used for the next calculations.

### Creating new aggregation functions
To create a new aggregation function other than just normal avg you need to create new classes implementing IAvgFunction **interface** and add them to **Calculator** service.
After that you can just set it in the appsettings. And don't forget to write tests for it.

---

## Database updates
To update the database please modify Models and Configurations in **DKExperiments.DB** project.
Then you'll be able to manipulate new migrations using standard commands (e.g. **Add-Migration**).
All migrations will be automatic, they will be applied once you start the API project. Don't update the database manually!

_Please also note that test database doesn't use any Volume so it will be completely clean on every single compose._

---

## Kubernetes

#### For this project I'll be describing all the steps for deployiung it on k8s on Windows with Docker Desktop installed.

For this to work you need to enable Kubernetes in Docker Desktop. You can do it in Settings.

First you need to activate private Docker images registry to be able to push your API image and keep it local/private.
You can do it by using the following command:
___docker run -d -p 5000:5000 --restart=always --name registry registry:2___

Next you'll need to build an image and push it to registry.
For that you need the following commands (keep the dot an the end):
___docker build --build-arg -t localhost:5000/dke-api:latest .___
___docker push localhost:5000/dke-api___

Now you can see your images in Docker and it is ready to be deployed via Kubernetes.
For that you need __k8s__ folder of the project. Navigate there in a terminal and follow next instructions.

Create persistent volume to store your database even if containers are removed:
_kubectl apply -f db-persistent-volume.yaml_
I didn't have any persistent volume so I had to create one. If you already have one you don't need to do this, you can change next files to use your volume.

__Create persistent volume claim:__ _kubectl apply -f db-persistent-volume-claim.yaml_

__Publish database:__ _kubectl apply -f db-stateful-set.yaml_

__Run service for your database to give access to it from inside a cluster:__ _kubectl apply -f db-service.yaml_

__Deploy an API:__ _kubectl apply -f api-deployment.yaml_

__Run service for your API to give access to it from inside _(and later from outside)_ a cluster:__ _kubectl apply -f api-service.yaml_

__Expose API port so you can actually access it from the outside:__ _kubectl expose deployment xm-api-deployment --type="LoadBalancer" --port=8080_


Now when everything is done you can access your API by opening the following link in your browser: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html).
