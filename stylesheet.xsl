<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html lang="en">

    <head>
      <title>Opal OPC Report</title>
      <meta charset="utf-8">
      </meta>
      <meta name="viewport" content="width=device-width, initial-scale=1">
      </meta>
      <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha3/dist/css/bootstrap.min.css" rel="stylesheet"
        integrity="sha384-KK94CHFLLe+nY2dmCWGMq91rCGa5gtU4mk92HdvYe+M/SXH301p5ILy+dN9+nJOZ" crossorigin="anonymous">
      </link>
      <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha3/dist/js/bootstrap.bundle.min.js"
        integrity="sha384-ENjdO4Dr2bkBIFxQpeoTz1HIcje39Wm4jDKdf19U8gI4ddQ3GYNS7NTKfAdVQSZe"
        crossorigin="anonymous"></script>
      <meta name="generator" content="OpalOPC">
      </meta>
    </head>

    <body class="d-flex h-100 text-bg-dark">
      <main class="px-3">
        <section class="text-center">
          <h1>Opal OPC Report</h1>
          <h2><xsl:value-of select="Report/Timestamp" /></h2>
        </section>

        <section>
          <h2>Navigation</h2>
        </section>

        <section>
          <h2>Scan summary</h2>
          <p>
            OpalOPC <xsl:value-of select="Report/Version" /> was initialized at <xsl:value-of
              select="Report/Timestamp" /> with these arguments:
            <br></br>
            <i><xsl:value-of select="Report/Command" /></i>
          </p>
          <p>
            Verbosity: XX
          </p>
          <p>
            Nmap done at Sun May 7 21:10:14 2023; 1 IP address (1 host up) scanned in 103.47 seconds
          </p>
        </section>

        <section>
          <xsl:for-each select="Report/Targets">
            <div>
              <h2><xsl:value-of select="Target/ApplicationName" /> (<xsl:value-of select="Target/ApplicationUri" />)
              </h2>
              <h3>Type: <xsl:value-of select="Target/Type" /></h3>
              <h3>Product Uri: <xsl:value-of select="Target/ProductUri" /></h3>
              <div>
                <h3>Servers</h3>
                <xsl:for-each select="Target/Servers">
                  <div>
                    <h3><xsl:value-of select="Server/DiscoveryUrl" /></h3>
                    <div>
                      <h4>Endpoints</h4>
                      <xsl:for-each select="Server/Endpoints">
                        <div>
                          <h5><xsl:value-of select="Endpoint/EndpointUrl" /></h5>
                          <div>
                            <table class="table text-bg-dark">
                              <thead>
                                <tr>
                                  <th>Issue</th>
                                  <th>Description</th>
                                </tr>
                              </thead>
                              <tbody>
                                <xsl:for-each select="Endpoint/Issues/Issue">
                                  <tr>
                                    <td><xsl:value-of select="Description" /></td>
                                    <td><xsl:value-of select="Description" /></td>
                                  </tr>
                                </xsl:for-each>
                              </tbody>
                            </table>
                          </div>
                          <p>
                            <a data-bs-toggle="collapse" href="#collapseExample" role="button" aria-expanded="false" aria-controls="collapseExample">
                              Endpoint details (click to expand)
                            </a>
                          </p>
                          <div class="collapse" id="collapseExample">
                            <div class="text-bg-dark card card-body">
                                <p>Security policy Uris</p>
                                <ul>
                                  <xsl:for-each select="Endpoint/SecurityPolicyUris/SecurityPolicyUri">
                                    <li><xsl:value-of select="." /></li>
                                  </xsl:for-each>
                                </ul>
                                <p>Message security Modes</p>
                                <ul>
                                  <xsl:for-each select="Endpoint/MessageSecurityModes/MessageSecurityMode">
                                    <li><xsl:value-of select="." /></li>
                                  </xsl:for-each>
                                </ul>
                                <p>User Token Policies</p>
                                <ul>
                                  <xsl:for-each select="Endpoint/UserTokenPolicyIds/UserTokenPolicy">
                                    <li><xsl:value-of select="." /></li>
                                  </xsl:for-each>
                                </ul>
                                <p>User Token Types</p>
                                <ul>
                                  <xsl:for-each select="Endpoint/UserTokenTypes/UserTokenType">
                                    <li><xsl:value-of select="." /></li>
                                  </xsl:for-each>
                                </ul>
                              </div>
                            </div>
                          </div>
                      </xsl:for-each>
                    </div>
                  </div>
                </xsl:for-each>
              </div>
            </div>
          </xsl:for-each>
        </section>
      </main>
    </body>

    </html>
  </xsl:template>
</xsl:stylesheet>