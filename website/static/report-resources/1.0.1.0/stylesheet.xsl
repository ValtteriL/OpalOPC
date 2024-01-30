<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" encoding="UTF-8" doctype-public="-//W3C//DTD HTML 4.01//EN"
    doctype-system="http://www.w3.org/TR/html4/strict.dtd" />
  <xsl:template match="/">
    <html
      lang="en">

      <head>
        <meta charset="utf-8"></meta>
        <meta name="viewport" content="width=device-width, initial-scale=1"></meta>
        <title>OpalOPC Report</title>

        <meta name="generator" content="OpalOPC"></meta>

        <!-- Link favicon CSS -->
        <link rel="icon"
          href="https://opalopc.com/report-resources/1.0.1.0/assets/images/favicon.png"
          type="image/png" sizes="16x16"></link>

        <!-- Link Bootstrap CSS -->
        <link href="https://opalopc.com/report-resources/1.0.1.0/assets/css/bootstrap.min.css"
          rel="stylesheet"></link>

        <!-- Link Fonts CSS -->
        <link rel="preconnect" href="https://fonts.googleapis.com"></link>
        <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin=""></link>
        <link
          href="https://fonts.googleapis.com/css2?family=Inter:wght@100;200;300;400;500;600;700;800;900&#38;display=swap"
          rel="stylesheet"></link>

        <!-- Link Custom CSS -->
        <link href="https://opalopc.com/report-resources/1.0.1.0/assets/scss/style.css"
          rel="stylesheet"></link>

      </head>

      <body dir="ltr">
        <!-- Start Main  -->

        <!-- Start Header  -->
        <header>
          <div class="container">

            <div class="logo">
              <img src="https://opalopc.com/report-resources/1.0.1.0/assets/images/logo.svg"
                alt=""></img>
            </div>

            <div class="headings">
              <h1>OpalOPC Report <span>
                  <xsl:value-of select="Report/StartTime" />
                </span>
              </h1>
            </div>


          </div>

        </header>
        <!-- ./ End Header  -->
        <div class="container">
          <hr class="separator"></hr>
        </div>
        <!-- Start Main -->
        <main>

          <!-- Start Welcome Text  -->
          <section class="opal-summary">
            <div class="container">
              <div class="headings">
                <h2>Scan summary</h2>
              </div>
              <div class="scan-summary-box">
                <div class="paragraph">
                  <p>OpalOPC <xsl:value-of select="Report/Version" /> was initialized at <xsl:value-of
                      select="Report/StartTime" /> with these arguments: <br></br>
                    <xsl:value-of
                      select="Report/Command" /></p>
                  <p class="mb-0">
                    <xsl:value-of select="Report/RunStatus" />
                  </p>
                </div>
              </div>
            </div>
          </section>


          <section class="opal-summary">
            <div class="container">
              <div class="headings">
                <h2>Application summary</h2>
              </div>
              <div class="opal-summary-table">
                <table class="table">
                  <thead>
                    <tr>
                      <th>#</th>
                      <th class="w-20">Application Name</th>
                      <th>Application type</th>
                      <th>Discovery URLs</th>
                      <th>Issues</th>
                      <th>Errors</th>
                    </tr>
                  </thead>
                  <tbody>
                    <xsl:for-each select="Report/Targets/Target">
                      <tr>
                        <td>
                          <xsl:element name="a">
                            <xsl:attribute name="href">#<xsl:value-of select="position()" />
                            </xsl:attribute>
                            <xsl:value-of
                              select="position()" />
                          </xsl:element>
                        </td>
                        <td>
                          <xsl:value-of select='ApplicationName' />
                        </td>
                        <td>
                          <xsl:value-of select="Type" />
                        </td>
                        <td>
                          <xsl:value-of select='count(Servers/Server)' />
                        </td>
                        <td>
                          <xsl:value-of
                            select='count(Servers/Server/Endpoints/Endpoint/Issues/Issue)' />
                        </td>
                        <td>
                          <xsl:value-of select='count(Servers/Server/Errors/Error)' />
                        </td>
                      </tr>
                    </xsl:for-each>
                  </tbody>
                </table>
              </div>
            </div>
          </section>

          <!-- Use this page break as per your content when you want to break page for PDF -->
          <div style="page-break-before:always"></div>


          <section class="opal-summary">
            <div class="container">
              <div class="headings">
                <h2>Results Summary <span>(<xsl:value-of select='count(Report/Targets/Target)' />
    Applications found )</span></h2>
              </div>
              <div class="opal-summary-table">
                <table class="table">

                  <tbody>
                    <tr>
                      <td class="bg-severity">Severity levels</td>
                      <td class="p-0 border-0">
                        <table class="table mb-0">

                          <thead class="results-bar">
                            <tr>
                              <th>
                                <div class="row">
                                  <div class="col">
                                    <div class="results info">
                                      <span></span>
                                      <span></span>
                                      <span></span>
                                      <span></span>
                                      <span></span>
                                    </div>
                                  </div>
                                  <div class="col p-0 justify-content-start">Info</div>
                                </div>

                              </th>
                              <th>
                                <div class="row">
                                  <div class="col">
                                    <div class="results low">
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span></span>
                                      <span></span>
                                      <span></span>
                                    </div>
                                  </div>
                                  <div class="col p-0">Low</div>
                                </div>

                              </th>
                              <th>
                                <div class="row">
                                  <div class="col">
                                    <div class="results medium">
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span></span>
                                      <span></span>
                                    </div>
                                  </div>
                                  <div class="col p-0">Medium</div>
                                </div>

                              </th>
                              <th>
                                <div class="row">
                                  <div class="col">
                                    <div class="results high">
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span></span>
                                    </div>
                                  </div>
                                  <div class="col p-0">High</div>
                                </div>

                              </th>
                              <th>
                                <div class="row">
                                  <div class="col">
                                    <div class="results critical">
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span class="active"></span>
                                      <span class="active"></span>
                                    </div>
                                  </div>
                                  <div class="col p-0">Critical</div>
                                </div>

                              </th>
                            </tr>
                            <tr>
                              <td>0</td>
                              <td>[0,1-3,9]</td>
                              <td>[4,0-6,9]</td>
                              <td>[7,0-8,9]</td>
                              <td>[9,0-10,0]</td>
                            </tr>

                          </thead>
                        </table>

                      </td>
                    </tr>

                  </tbody>
                </table>
              </div>
            </div>
          </section>


          <section class="opal-summary">
            <div class="container">
              <div class="opal-summary-table opal-summary-detail">

                <xsl:for-each select="Report/Targets/Target">
                  <xsl:element name="table">
                    <xsl:attribute name="id"><xsl:value-of select="position()" />
                    </xsl:attribute>
                    <xsl:attribute
                      name="class">table mb-4</xsl:attribute>
                    <tbody>
                      <tr>
                        <td class="col-numbering bg-01 border-0">
                          <xsl:value-of select="position()" />
                        </td>
                        <td class="bg-results p-5 border-0">
                          <table class="table table-striped mb-5">
                            <tbody>
                              <tr>
                                <td class="w-25">Application name</td>
                                <td class="text-start">
                                  <xsl:value-of select='ApplicationName' />
                                </td>
                              </tr>
                              <tr>
                                <td>Application type</td>
                                <td class="text-start">
                                  <xsl:value-of select="Type" />
                                </td>
                              </tr>
                              <tr>
                                <td>Application URI</td>
                                <td class="text-start">
                                  <xsl:value-of select="ApplicationUri" />
                                </td>
                              </tr>
                              <tr>
                                <td>Product URI</td>
                                <td class="text-start">
                                  <xsl:value-of
                                    select="ProductUri" />
                                </td>
                              </tr>
                              <tr>
                                <td>Errors</td>
                                <td class="text-start">
                                  <xsl:value-of select='count(Servers/Server/Errors/Error)' />
                                </td>
                              </tr>
                            </tbody>
                          </table>

                          <table class="table discovery mb-0">
                            <thead>
                              <tr>
                                <th class="w-25">Discovery URL</th>
                                <th>Issue type</th>
                                <th>Plugin Id</th>
                                <th>Severity</th>
                              </tr>
                            </thead>
                            <tbody>
                              <xsl:for-each select="Servers/Server">
                                <tr class="discoveryurl">
                                  <xsl:element name="td">
                                    <xsl:if test="position() = last()">
                                      <xsl:attribute
                                        name="class">text-start br-rb</xsl:attribute>
                                    </xsl:if>
                                    <xsl:if
                                      test="position() != last()">
                                      <xsl:attribute
                                        name="class">text-start br-b0</xsl:attribute>
                                    </xsl:if>
                                      <xsl:attribute
                                      name="rowspan">
                                      <xsl:if
                                        test="count(Errors/Error) = 0 and count(Endpoints/Endpoint/Issues/Issue) = 0">
    2</xsl:if>
                                      <xsl:if
                                        test="count(Errors/Error) != 0 or count(Endpoints/Endpoint/Issues/Issue) != 0">
                                        <xsl:value-of
                                          select='count(Endpoints/Endpoint/Issues/Issue)+count(Errors/Error)+1' />
                                      </xsl:if>
                                    </xsl:attribute>
                                      <xsl:value-of
                                      select="DiscoveryUrl" />
                                  </xsl:element>
                                </tr>

                                <xsl:for-each
                                  select="Endpoints/Endpoint/Issues/Issue">

                                  <xsl:variable name="sev">
                                    <xsl:choose>
                                      <xsl:when test="Severity &gt;= 9">
                                        <td>
                                          <div class="row">
                                            <div class="col pe-0 text-start">Critical (<xsl:value-of
                                                select="Severity" />)</div>
                                            <div class="col">
                                              <div class="results critical">
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                              </div>
                                            </div>
                                          </div>
                                        </td>
                                      </xsl:when>
                                      <xsl:when test="Severity &gt;= 7">
                                        <td>
                                          <div class="row">
                                            <div class="col pe-0 text-start">Hight (<xsl:value-of
                                                select="Severity" />)</div>
                                            <div class="col">
                                              <div class="results high">
                                                <span class="active"></span>
                                                <span class="active"></span>
                                                <span class="active"></span>
                                                <span class="active"></span>
                                                <span></span>
                                              </div>
                                            </div>
                                          </div>
                                        </td>
                                      </xsl:when>
                                      <xsl:when test="Severity &gt;= 4">
                                        <td>
                                          <div class="row">
                                            <div class="col pe-0 text-start">Medium (<xsl:value-of
                                                select="Severity" />)</div>
                                            <div class="col">
                                              <div class="results medium">
                                                <span class="active"></span>
                                                <span class="active"></span>
                                                <span class="active"></span>
                                                <span class="active"></span>
                                                <span></span>
                                              </div>
                                            </div>
                                          </div>
                                        </td>
                                      </xsl:when>
                                      <xsl:when test="Severity &gt;= 0.1">
                                        <td>
                                          <div class="row">
                                            <div class="col pe-0 text-start">Low (<xsl:value-of
                                                select="Severity" />)</div>
                                            <div class="col">
                                              <div class="results low">
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                              </div>
                                            </div>
                                          </div>
                                        </td>
                                      </xsl:when>
                                      <xsl:when test="Severity &gt;= 0">
                                        <td>
                                          <div class="row">
                                            <div class="col pe-0 text-start">Info (<xsl:value-of
                                                select="Severity" />)</div>
                                            <div class="col">
                                              <div class="results info">
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                                <span></span>
                                              </div>
                                            </div>
                                          </div>
                                        </td>
                                      </xsl:when>
                                    </xsl:choose>
                                  </xsl:variable>

                                  <tr>
                                    <td class="text-start">
                                      <xsl:value-of select="Name" />
                                    </td>
                                    <td>
                                      <xsl:element name="a">
                                        <xsl:attribute name="href"> https://opalopc.com/docs/plugin-<xsl:value-of
                                            select="PluginId" />
                                        </xsl:attribute>
                                        <xsl:attribute
                                          name="target"> _blank </xsl:attribute>
                                        <xsl:value-of
                                          select="PluginId" />
                                      </xsl:element>
                                    </td>
                                    <xsl:copy-of select="$sev" />
                                  </tr>
                                </xsl:for-each>

                                <xsl:for-each
                                  select="Errors/Error">
                                  <tr>
                                    <td class="text-start" colspan="3">
                                      <ul class="error">
                                        <li>
                                          <xsl:value-of select="Message" />
                                        </li>
                                      </ul>
                                    </td>
                                  </tr>
                                </xsl:for-each>

                                <xsl:if
                                  test="count(Errors/Error) = 0 and count(Endpoints/Endpoint/Issues/Issue) = 0">
                                  <tr>
                                    <td class="text-start" colspan="3">
                                      <div class="no-issue-found">
                                        <p>No issues found.</p>
                                      </div>
                                    </td>
                                  </tr>
                                </xsl:if>


                              </xsl:for-each>
                            </tbody>
                          </table>

                        </td>
                      </tr>

                    </tbody>
                  </xsl:element>
                </xsl:for-each>

              </div>
            </div>
          </section>


          <!-- ./ End Welcome Text  -->


        </main>
        <!-- ./ End Main  -->
        <footer>
          <div class="container">
            <div class="footer-text">
              <div class="row">
                <div class="col">
                  <div class="paragraph">
                    <p>
                      <a target="_blank" href="https://opalopc.com">opalopc.com</a>
                    </p>
                  </div>
                </div>
                <div class="col text-end">
                  <div class="paragraph">
                    <p>Missing features? Facing a bug? <a target="_blank"
                        href="https://opalopc.com/contact-us/">Give us feedback!</a></p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </footer>
        <!-- ./ End Main Body  -->

        <!-- JS -->
        <script src="https://opalopc.com/report-resources/1.0.1.0/assets/js/popper.min.js"></script>
        <script src="https://opalopc.com/report-resources/1.0.1.0/assets/js/bootstrap.min.js"></script>
        <script src="https://opalopc.com/report-resources/1.0.1.0/assets/js/jquery.min.js"></script>


      </body>

    </html>
  </xsl:template>
</xsl:stylesheet>