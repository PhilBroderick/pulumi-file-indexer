# Pulumi Serverless Application

This repository demonstrates a basic serverless function that is triggered by file uploads to a storage bucket, in turn creating an index in a database table, which is simply the name of the file including the time it was uploaded.

All of the infrastructure for this application is defined and deployed using Pulumi.

## Requirements

* Serverless application that processes uploads to a storage bucket and builds an index of the files in a database table

* Use Azure/AWS/GCP to host application and infrastructure

* Pulumi infrastructure including:
  * A storage bucket
  * A database table
  * Serverless function
  * Component resource encapsulating two or more resources

## Success Criteria

I should be able to upload a file to the bucket and see the database entry get created.

## Technologies

* Azure:
  * SQL (database table)
  * Blob storage (storage bucket)
  * Function (serverless function)

* C#/.NET 8 for Pulumi infrastructure code.

* GitHub Actions for CI/CD.

## Considerations/Future Improvements

Given the time/requirement constraints of this project, some fair assumptions were made, as well
as some technology decisions to speed up development time. If more time were dedicated to this project, here are just some of the improvements that can/would be made:

* Secure access between storage account > Function > Database. Currently, these use connection strings (stored in `appsettings`), however, to improve security and remove the need for storing these credentials,
  Azure Private Endpoints/Managed Identities could be utilised so the Function acts as an identity, with the required permissions to access the storage/database resources over the Azure backbone instead of via the public web.
* In a similar vein, the blob container is currently set with the `blob` access level, meaning anyone can upload files to this container. This is far from ideal. A first step would be to make this `private`, and provide SAS (scoped tokens that expire) tokens
  to facilitate the file upload functionality. Furthermore, this could be completely abstracted behind a simple proxy service, creating an even more secure surface.
* There is currently no retry logic between the Function and database, meaning any transient/more serious failures connecting to the database would cause the file upload process to error out.
  A resiliency strategy should be put in place as working in the Cloud inherently introduces transient errors that should be handled.
* Depending on pricing/performance requirements, the Function app could be moved up a tier, or moved down to Consumption. With this comes cold starts, but from a cost perspective is better than always paying for the compute.
* Likewise, the Database is running on a `Standard` tier currently. Again this could be scaled up, or moved down to a free tier.
* Alternatively, as a storage account is already being utilised, a Table could be used for the datastore, improving performance at a low cost, as well as maintenance overhead.
* The GitHub Actions could be smarter and only build code/preview infrastructure changes if the given files have changed. This is fairly arbitrary with the `push` trigger.
* Instead of using a local `Pulumi.yaml` file for configuration, utilising something like ESC would allow for much easier configuration management, improving security and sync of changes between deployments.

## Running Locally

To run the Function locally, the following pre-requisites are in place:
* SQL Server must be running in Docker, with `SqlConnectionString` environment variable defined
* Database has been created that matches connection string
* EF Migrations ran against Docker SQL (Handled by Function startup)
* .NET 7 + Azure tools installed
* Azure Storage emulator must be running locally (Azurite).