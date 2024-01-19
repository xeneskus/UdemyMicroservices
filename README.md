# UdemyMicroservices
In this project, a clone of popular online education platform Udemy.com is created using microservice architecture.
![114802958-42c15d80-9da7-11eb-8391-ba0abf87a1b1](https://github.com/xeneskus/UdemyMicroservices/assets/94754002/5bd52dac-baa1-4453-adc3-2cbfdd1dc7b4)
<h2>Features :</h2>
<li>Synchronous and asynchronous communication between microservices using <del>.Net5</del> .Net 6</li>
<li>Implementation of OAuth 2.0 and OpenID Connect protocols in Microservices architecture</li>
<li>Using Eventual Consistency model to ensure consistency in databases of microservices</li>
<li>Dockerize all microservices using docker-compose</li>
<li>Asp.Net Core MVC Client for UI</li>
<li>User Register via IdentityServer4</li>
<li>Asynchronous order and payment process</li>
<li>Asynchronous course name update process between catalog, order and basket microservices</li>
<h2>Microservices :</h2>
<h3>Catalog Microservice</h3>
<li>MongoDb (Database)</li>
<li>One-To-Many/One-To-One relation</li>
<h3>Basket Microservice</h3>
<li>RedisDB (Database)</li>
<h3>Discount Microservice</h3>
<li>PostgreSQL (Database)</li>
<h3>Order Microservice</h3>
<li>SqlServer (Database)</li>
<li>Domain Driven Design</li>
<li>CQRS (MediatR library)</li>
<h3>FakePayment Microservice</h3>
It is Microservice that is responsible for payment processes.
<h3>Identity Microservice</h3>
<li>SqlServer(Database)</li>
<li>Protect Microservices using Access Token</li>
<li>OAuth 2.0 / OpenID Connect protocols</li>
<h3>PhotoStock Microservice</h3>
It is the microservice that is responsible for keeping and presenting course photos.
<h3>API Gateway</h3>
<li>Ocelot Library</li>
<h3>Message Broker</h3>
RabbitMQ is used as message queue system. MassTransit library is also used for microservices to communicate with RabbitMQ.
<li>RabbitMQ (MassTransit Library)</li>
<h3>Asp.Net Core MVC Microservice</h3>
It is the UI microservice that displays the data received from Microservices to the user and is responsible for interacting with the user.
