# Pulumi Serverless Application

This repository demonstrates a basic serverless function that is triggered by file uploads to a storage bucket, in turn creating an index in a database table.

All of the insfrastructure for this application is defined and deployed using Pulumi.

## Requirements

* Serverless application that processes uploads to a storage bucket and builds an index of the files in a database table

* Use Azure/AWS/GCP to host application and infrastructure

* Pulumi infrastructure including:
  * A storage bucket
  * A database table
  * Serverless function
  * Component resource encapsulating two or more resources

## Success Criteria

I should be able to upload a file to the buekt and see the database entry get created.

## Technologies

* Azure:
  * Table storage (database table)
  * Blob storage (storage bucket)
  * Function (serverless function)

* C#/.NET 8.

* GitHub Actions for CI/CD.

## Running Locally

To run the Function locally, the following pre-requisites are in place:
* SQL Server must be running in Docker, with `SqlConnectionString` environment variable defined
* Database has been created that matches connection string
* EF Migrations ran against Docker SQL (Handled by Function startup)
* .NET 7 + Azure tools installed
* Azure Storage emulator must be running locally (Azurite).