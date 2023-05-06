<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>

    <head>
      <title>Opal OPC Report</title>
    </head>

    <body>
      <h1>Opal OPC Report</h1>
      <h2><xsl:value-of select="Report/Timestamp" /></h2>
      <h3>Opal OPC Version: <xsl:value-of select="Report/Version" /></h3>
      <h3>Command: opalopc -v XXXXX</h3>


      <xsl:for-each select="Report/Targets">
        <div>
          <h1>Application name: <xsl:value-of select="Target/ApplicationName" /></h1>
          <h2>Type: <xsl:value-of select="Target/Type" /></h2>
          <h2>Applicaton Uri: <xsl:value-of select="Target/ApplicationUri" /></h2>
          <h2>Product Uri: <xsl:value-of select="Target/ProductUri" /></h2>
          <div>
            <h2>Servers</h2>
            <xsl:for-each select="Target/Servers">
              <div>
                <h3>Discovery Uri: <xsl:value-of select="Server/DiscoveryUrl" /></h3>
                <div>
                  <h3>Endpoints</h3>
                  <xsl:for-each select="Server/Endpoints">
                    <div>
                      <h4>Endpoint Uri: <xsl:value-of select="Endpoint/EndpointUrl" /></h4>
                      <div>
                        <h4>Security policy Uris</h4>
                        <ul>
                          <xsl:for-each select="Endpoint/SecurityPolicyUris/SecurityPolicyUri">
                            <li><xsl:value-of select="." /></li>
                          </xsl:for-each>
                        </ul>
                      </div>
                      <div>
                        <h4>Message security Modes</h4>
                        <ul>
                          <xsl:for-each select="Endpoint/MessageSecurityModes/MessageSecurityMode">
                            <li><xsl:value-of select="." /></li>
                          </xsl:for-each>
                        </ul>
                      </div>
                      <div>
                        <h4>User Token Policies</h4>
                        <ul>
                          <xsl:for-each select="Endpoint/UserTokenPolicyIds/UserTokenPolicy">
                            <li><xsl:value-of select="." /></li>
                          </xsl:for-each>
                        </ul>
                      </div>
                      <div>
                        <h4>User Token Types</h4>
                        <ul>
                          <xsl:for-each select="Endpoint/UserTokenTypes/UserTokenType">
                            <li><xsl:value-of select="." /></li>
                          </xsl:for-each>
                        </ul>
                      </div>
                      <div>
                        <h5>Issues</h5>
                        <xsl:for-each select="Endpoint/Issues/Issue">
                          <div>
                            <h6>Title: <xsl:value-of select="Description" /></h6>
                            <p>Description: <xsl:value-of select="Description" /></p>
                          </div>
                        </xsl:for-each>
                      </div>
                    </div>
                  </xsl:for-each>
                </div>
              </div>
            </xsl:for-each>
          </div>
        </div>
      </xsl:for-each>
    </body>

    </html>
  </xsl:template>
</xsl:stylesheet>