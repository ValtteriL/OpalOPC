
          <section>
            <h2>Results</h2>
            <xsl:for-each select="Report/Targets/Target">
              <div>
                <xsl:element name="h3">
                  <xsl:attribute name="id">
                    <xsl:value-of select='ApplicationName' />
                  </xsl:attribute>
                <xsl:value-of
                    select='ApplicationName' />
                <span class="mx-2 badge bg-secondary">
                    <xsl:value-of select="Type" />
                  </span>
                </xsl:element>
                <p>Application Uri: <xsl:value-of select="ApplicationUri" />
                <br></br> Product Uri: <xsl:value-of
                    select="ProductUri" />
                <br></br> Errors: <xsl:value-of
                    select="count(Servers/Server/Errors/Error)" />
                </p>
                <div>
                  <xsl:for-each select="Servers/Server">
                    <div>
                      <h5>
                        <xsl:value-of select="DiscoveryUrl" />
                      </h5>
                      <xsl:if test="count(Errors/Error) &gt; 0">
                        <div class="text-danger">
                          <h6>Errors</h6>
                          <ul>
                            <xsl:for-each select="Errors/Error">
                              <li>
                                <xsl:value-of select="Message" />
                              </li>
                            </xsl:for-each>
                          </ul>
                        </div>
                      </xsl:if>
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
                                          <td class="table-danger">Critical (<xsl:value-of
                                              select="Severity" />)</td>
                                        </xsl:when>
                                        <xsl:when test="Severity &gt;= 7">
                                          <td class="table-danger">High (<xsl:value-of
                                              select="Severity" />)</td>
                                        </xsl:when>
                                        <xsl:when test="Severity &gt;= 4">
                                          <td class="table-warning">Medium (<xsl:value-of
                                              select="Severity" />)</td>
                                        </xsl:when>
                                        <xsl:when test="Severity &gt;= 0.1">
                                          <td class="table-warning">Low (<xsl:value-of
                                              select="Severity" />)</td>
                                        </xsl:when>
                                        <xsl:when test="Severity &gt;= 0">
                                          <td class="table-info">Info (<xsl:value-of
                                              select="Severity" />)</td>
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
                                        <xsl:attribute
                                            name="target"> _blank </xsl:attribute>
                                        <xsl:value-of
                                            select="PluginId" />
                                        </xsl:element>
                                      </td>
                                      <td>
                                        <xsl:value-of select="Name" />
                                      </td>
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
          <footer
            class="d-flex flex-wrap justify-content-between align-items-center py-3 my-4 border-top">
            <p class="col-md-4 mb-0 text-muted">opalopc.com</p>
            <p>Missing features? Facing a bug? <a target="_blank"
                href="https://opalopc.com/contact-us/">Give us
                feedback!</a></p>
          </footer>
        </main>
      </body>

    </html>