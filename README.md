iis2tomcat
==========

-- Hands-down the easiest connector to deploy for IIS to Tomcat connectivity -- ;o)

This is the modern method of connecting IIS and Apache Tomcat. Most likely to use a Railo, Lucee or JSP driven backend.
The BonCode AJP (Apache JServ Protocol version 1.3) Connector uses already existing pathways to connect to Apache Tomcat. 
The AJP implementation is generic and will work to connect IIS with any AJP server such as Jboss, web-methods, Jetty etc.
In general it is a preference question how you connect IIS to Tomcat, though, there are several advantages with the BonCode connector vs the old ISAPI connector:
*   no ISAPI code, IIS6 vestiges or backward compatibility elements needed
*   does not block or hinder IIS functionality or slows unrelated requests
*   all managed code for IIS7+ using the modern extensibility framework
*   works on IIS5.1, IIS6 and IIS7, IIS7.5, IIS8+ (Windows XP through Windows 10, Windows Server 2003-2012)
*   speed, throughput, and stability improvements 
*   configuration in IIS UI
*   no virtual directories and virtual mappings needed
*   configuration can be inherited to sub-paths and virtual sites
*   easy install/uninstall
*   support partial stream sending to browser (automatic flushing) with faster response to client
*   support both 32/64 bit of Windows with same process and files
*   transfer of all request headers
*   build-in simple-security for web-administration pages (Tomcat, Railo, OpenBD, ColdFusion)
*   IP6 support
*   Additional HTTP headers data is passed to Tomcat servlet container (previously unavailable)
*   Improved transfer of SSL data to Tomcat servlet container
*   Support improved translation of load balancer headers to determine correct client IP 
*   Support client fingerprint mechanism for use with safer sessions
*   Support for Adobe Coldfusion 10,11 AJP dialect
*   Support for Lucee, Railo, and OpenBD CFML Engines 
*   Support for alternate Path-Info header transmission via AJP

If you were using a proxy or URL rewrite engine you would also benefit from:
*   Fully integrated SSL to Servlet container
*   Tomcat threading awareness (will not overload Tomcat and drop connections unnecessarily)
*   Your servlets and scripts will receive correct HTTP header/URL/IP information for processing
*   reduced traffic and processing on both IIS and tomcat sides
*   allows you to connect to multiple tomcat instances from within one IIS site without interfering with ISAPI connector, e.g. Shibboleth and ColdFusion 10/Railo

YouTube Videos:
http://tomcatiis.riaforge.org/blog/index.cfm/2011/4/10/Youtube-Videos


## Version History:

Expanded version history is in the `Readme Notes.txt` file in this project

## More Documentation and Support


Full documentation is available in the download package from [github releases site](https://github.com/Bilal-S/iis2tomcat/releases). Download zip and look in `BonCode_Tomcat_Connector_Manual.pdf` file. 

The documentation also contains manual installation instruction, however, using automated installer contained in package is recommended.

As usual any feedback is appreciated. Please use the [github issues site](https://github.com/Bilal-S/iis2tomcat/issues) to leave feedback or open issues.
