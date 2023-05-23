<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" encoding="UTF-8" doctype-public="-//W3C//DTD HTML 4.01//EN"
    doctype-system="http://www.w3.org/TR/html4/strict.dtd" />
  <xsl:template match="/">
    <html lang="en">

    <head>
      <title>OpalOPC Report</title>
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
          <h1>OpalOPC Report</h1>
          <h2><xsl:value-of select="Report/StartTime" /></h2>
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
          <h2>Application summary</h2>
          <table class="table table-hover">
            <thead class="table-bordered table-light">
              <tr>
                <th>Application</th>
                <th>Type</th>
                <th>DiscoveryURIs</th>
                <th>Issues</th>
                <th>Errors</th>
              </tr>
            </thead>
            <tbody>
              <xsl:for-each select="Report/Targets/Target">
                <tr>
                  <td>
                    <xsl:element name="a">
                      <xsl:attribute name="href">
                        #<xsl:value-of select='ApplicationName' />
                      </xsl:attribute>
                      <xsl:value-of select='ApplicationName' />
                    </xsl:element>
                  </td>
                  <td><xsl:value-of select="Type" /></td>
                  <td><xsl:value-of select='count(Servers/Server)' /></td>
                  <td><xsl:value-of select='count(Servers/Server/Endpoints/Endpoint/Issues/Issue)' /></td>
                  <td><xsl:value-of select='count(Errors/Error)' /></td>
                </tr>
              </xsl:for-each>
            </tbody>
          </table>
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
                Product Uri: <xsl:value-of select="ProductUri" />
              </p>
              <xsl:if test="count(Errors/Error) &gt; 0">
                <div class="text-danger">
                  <h4>Errors</h4>
                  <ul>
                    <xsl:for-each select="Errors/Error">
                      <li><xsl:value-of select="Message" /></li>
                    </xsl:for-each>
                  </ul>
                </div>
              </xsl:if>
              <div>
                <xsl:for-each select="Servers/Server">
                  <div>
                    <h5><xsl:value-of select="DiscoveryUrl" /></h5>
                    <div>
                      <xsl:for-each select="Endpoints/Endpoint">
                        <div>
                          <div>
                            <table class="table table-hover">
                              <thead class="table-bordered table-light">
                                <tr>
                                  <th>Severity</th>
                                  <th>Plugin Id</th>
                                  <th>Name</th>
                                </tr>
                              </thead>
                              <tbody>
                                <xsl:for-each select="Issues/Issue">
                                  <xsl:variable name="sev">
                                    <xsl:choose>
                                      <xsl:when test="Severity &gt;= 9">
                                        <td class="table-danger">Critical (<xsl:value-of select="Severity" />)</td>
                                      </xsl:when>
                                      <xsl:when test="Severity &gt;= 7">
                                        <td class="table-danger">High (<xsl:value-of select="Severity" />)</td>
                                      </xsl:when>
                                      <xsl:when test="Severity &gt;= 4">
                                        <td class="table-warning">Medium (<xsl:value-of select="Severity" />)</td>
                                      </xsl:when>
                                      <xsl:when test="Severity &gt;= 0.1">
                                        <td class="table-warning">Low (<xsl:value-of select="Severity" />)</td>
                                      </xsl:when>
                                      <xsl:when test="Severity &gt;= 0">
                                        <td class="table-info">Info (<xsl:value-of select="Severity" />)</td>
                                      </xsl:when>
                                    </xsl:choose>
                                  </xsl:variable>

                                  <tr>
                                    <xsl:copy-of select="$sev" />
                                    <td>
                                      <xsl:element name="a">
                                        <xsl:attribute name="href">
                                          https://opalopc.com/docs/plugin-<xsl:value-of select="PluginId" />
                                        </xsl:attribute>
                                        <xsl:attribute name="target">
                                          _blank
                                        </xsl:attribute>
                                        <xsl:value-of select="PluginId" />
                                      </xsl:element>
                                    </td>
                                    <td><xsl:value-of select="Name" /></td>
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
          <p>Missing features? Facing a bug? <a target="_blank" href="https://forms.gle/FWFuAmZs3H32jeNi6">Give us
              feedback!</a></p>
        </footer>
      </main>
    </body>

    </html>
  </xsl:template>
</xsl:stylesheet>