---
title: How to Continuously Test Security with Jenkins
description: XXXXXXXXXXXXXXXXXXXXX
keywords: [how-to, jenkins, cicd, automation]
sidebar_position: 2
---


Jenkins is a popular open-source automation server.
Commonly used as a integration and continuous delivery (CI/CD) tool, Jenkins helps to automate testing and deployment of code.

Security testing is an important, but time-consuming process of ensuring the security of products.
This tutorial shows how you can automate this process for your OPC UA server using Jenkins and OpalOPC.
The result is continuous security testing of your server, which allows you to catch any issues as early as possible.


## Prerequisites

- [Jenkins](https://www.jenkins.io/download/) installed and running
- The following plugins installed in Jenkins:
    - [Docker Pipeline](https://plugins.jenkins.io/docker-workflow/)
    - [Pipeline](https://plugins.jenkins.io/workflow-aggregator/)
    - [Docker Commons](https://plugins.jenkins.io/docker-commons/)
- Your OPC UA server source code is in Git
    - We use [a fork](https://github.com/ValtteriL/UA-.NETStandard) of UA-.NETStandard in Github for this purpose. We will be testing the [Console Reference Server](https://github.com/ValtteriL/UA-.NETStandard/tree/master/Applications/ConsoleReferenceServer).
- Jenkins is configured with credentials required to pull the Git repository
- You know how to build and run your OPC UA server

## Overview

We will be creating a pipeline that clones the Git repository, builds and runs the OPC UA server, scans it with OpalOPC, and finally archives the scanning report for us to download.



We avoid installing any additional software on the Jenkins server by using Docker containers for the build and scanning processes.

:::note

You can also install OpalOPC directly on the Jenkins runner. However, this is not recommended as it makes the Jenkins server harder to maintain.

:::

## Create a pipeline

1. Login to Jenkins
2. Create a new pipeline in Jenkins, and configure it to fetch the Jenkinsfile from your Git repository.
KUVA

:::note

You can also use the `Pipeline script` setting to write the pipeline directly in Jenkins. However, this is not recommended as it makes it difficult to version control the pipeline.

:::

3. Select `Build Now` to run the pipeline. Verify you get the error `ERROR: Unable to find Jenkinsfile from git`
KUVA
4. Create a new file called `Jenkinsfile` in the root of your Git repository with the following content:

```groovy
asdasd
````

5. Create a new file called `Dockerfile.opalopc`, also in the root, with the following contents:

```Dockerfile
FROM ubuntu:jammy

# Install dependencies
RUN apt-get update && apt-get install -y \
    curl \
    libicu70

# Install opalopc http://opalopc.com/docs/get-started/install
RUN curl -LO "https://dl.opalopc.com/release/$(curl -L -s https://dl.opalopc.com/release/stable.txt)/bin/linux/amd64/opalopc" \
    && install -o root -g root -m 0755 opalopc /usr/local/bin/opalopc
```

6. Push the changes to your Git repository
7. Configure a new secret text credential in Jenkins with the name `opalopc-license-key` and the value of your OpalOPC license key.
KUVA

## Get results

After the pipeline is configured, we can run it to get the testing report.

1. Select `Build Now` to run the pipeline. The first run will take a while as Jenkins needs to download and build docker images. Subsequent runs will be a lot faster.
2. Verify the pipeline finishes successfully
3. Select the reports below `Last Successful Artifacts` to download them. Note: the HTML report does not display correctly on Jenkins. Therefore you should download it on your machine for viewing. KUVA

## Configure continuous testing

Now that we have a working pipeline, we will configure it to run automatically when new code is pushed to the Git repository.

1. Open the pipeline in Jenkins
1. Select `Configure` in the left-hand menu
1. Scroll down to the `Build Triggers` section
1. Select `GitHub hook trigger for GITScm polling`
1. Save the configuration

Now every time you push new code to your Git repository, Jenkins will automatically run OpalOPC on the ReferenceServer. You simply need to check the pipeline results to see if there are any security issues.
 
:::note

The best practice is to configure webhooks in your Git repository to trigger the pipeline. This reduces the load on your Git server and ensures that the pipeline is triggered as soon as possible.

:::

## Conclusion

Automating security testing of your OPC UA servers with Jenkins and OpalOPC is a great way to ensure you always have the latest security assessment results at hand.
By tracking the results, you will be the first to learn when a change in codebase makes your server vulnerable.

Usually, the next step after automation is to configure centralized vulnerability management, which allows you to track the security of your servers over time.
This is especially important in large organizations with many servers.
We will cover this in a future tutorial.
