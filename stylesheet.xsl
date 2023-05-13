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

    <body class="d-flex h-100">
      <main class="container">
        <section class="text-center">
          <img src="opalopc-logo.png" class="img-fluid" alt="OpalOPC logo"></img>
          <h1>Opal OPC Report</h1>
          <h2><xsl:value-of select="Report/StartTime" /></h2>
        </section>

        <section>
          <h2>Navigation</h2>
          <xsl:for-each select="Report/Targets/Target">
            <div>
              <xsl:element name="a">
                <xsl:attribute name="href">
                  #<xsl:value-of select='ApplicationName' />
                </xsl:attribute>
                <xsl:value-of select='ApplicationName' />
              </xsl:element>
            </div>
          </xsl:for-each>
        </section>

        <section>
          <h2>Scan summary</h2>
          <p>
            OpalOPC <xsl:value-of select="Report/Version" /> was initialized at <xsl:value-of
              select="Report/StartTime" /> with these arguments:
            <br></br>
            <i><xsl:value-of select="Report/Command" /></i>
          </p>
          <p>
            <xsl:value-of select="Report/RunStatus" />
          </p>
        </section>

        <section>
          <h2>Results</h2>
          <xsl:for-each select="Report/Targets/Target">
            <div>
              <xsl:element name="h3">
                <xsl:attribute name="id">
                  <xsl:value-of select='ApplicationName' />
                </xsl:attribute>
                <xsl:value-of select='ApplicationName' />
                <span class="mx-2 badge bg-secondary"><xsl:value-of select="Type" /></span>
              </xsl:element>
              <p>Application Uri: <xsl:value-of select="ApplicationUri" />
                <br></br>
              Product Uri: <xsl:value-of select="ProductUri" /></p>
              <div>
                <h4>Servers</h4>
                <xsl:for-each select="Servers/Server">
                  <div>
                    <h5><xsl:value-of select="DiscoveryUrl" /></h5>
                    <div>
                      <xsl:for-each select="Endpoints/Endpoint">
                        <xsl:variable name="EndpointUrl"><xsl:value-of select="EndpointUrl" /></xsl:variable>
                        <div>
                          <div>
                            <table class="table table-hover">
                              <thead class="table-bordered table-light">
                                <tr>
                                  <th>Endpoint</th>
                                  <th>Issue</th>
                                  <th>Description</th>
                                </tr>
                              </thead>
                              <tbody>
                                <xsl:for-each select="Endpoints/Endpoint"></xsl:for-each>
                                <xsl:for-each select="Issues/Issue">
                                  <tr>
                                    <td><xsl:copy-of select="$EndpointUrl" /></td>
                                    <td class="table-danger"><xsl:value-of select="Title" /></td>
                                    <td><xsl:value-of select="Description" /></td>
                                  </tr>
                                </xsl:for-each>
                              </tbody>
                            </table>
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
        <footer class="d-flex flex-wrap justify-content-between align-items-center py-3 my-4 border-top">
          <p class="col-md-4 mb-0 text-muted">opalopc.com</p>
          <p>Missing features? Facing a bug? <a target="_blank" href="https://forms.gle/FWFuAmZs3H32jeNi6">Give us feedback!</a></p>
        </footer>
      </main>
    </body>

    </html>
  </xsl:template>
</xsl:stylesheet>