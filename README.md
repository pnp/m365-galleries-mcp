# Microsoft 365 Galleries MCP Servers

MCP Servers for the Microsoft 365 & Power Platform Community Samples Gallery

## Overview

This repository contains two implementations of the same Model Context Protocol (MCP) Server that provides access to the Microsoft 365 & Power Platform Community Samples Gallery. Both projects expose identical functionality but use different communication protocols:

## Projects

### 1. SampleGalleriesMCPServerStdio
- **Protocol**: STDIO (Standard Input/Output)
- **Use Case**: Ideal for local development and integration with tools that support STDIO-based MCP servers
- **Communication**: Uses standard input/output streams for message exchange

### 2. SampleGalleriesMCPServerHttp
- **Protocol**: HTTP Streaming
- **Use Case**: Perfect for web-based applications and services that require HTTP-based communication
- **Communication**: Uses HTTP streaming for real-time message exchange

## Functionality

Both MCP servers provide the same core functionality:
- Access to the Microsoft 365 & Power Platform Community Samples Gallery
- Search and retrieve community samples
- Browse samples by various criteria (author, product, keywords)
- Get detailed information about specific samples

## Getting Started

Choose the implementation that best fits your integration needs:
- Use **SampleGalleriesMCPServerStdio** for command-line tools and local development
- Use **SampleGalleriesMCPServerHttp** for web applications and HTTP-based integrations

Both servers connect to the same Microsoft 365 & Power Platform Community Samples Gallery API and provide identical data and functionality.
