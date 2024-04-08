import React from 'react';

const Critical = () => {
  return (
    <div className="critical-container">
        <div className="common-container">
            <div className="critical-wrapper">
                <h2>Ensure your protection.</h2>
                <p>Save time and effort.</p>
                <div className="findings-container" >
                    <div className="findings coverage">
                        <img src="/img/coverage.png" alt="coverage" />
                        <div className="findings-name">
                            <h3>Unbeatable coverage</h3>
                            <p>Unique and ever-evolving set of security tests that put you to the forefront of OPC UA security assessments and keep you there.</p>
                        </div>
                    </div>
                    <div className="findings security">
                        <img src="/img/security.png" alt="security" />
                        <div className="findings-name">
                            <h3>Headless Security</h3>
                            <p>Use any feature of OpalOPC from the comfort of your terminal. Automate scans with scripts and integrate them to your CI/CD pipelines.</p>
                        </div>
                    </div>
                    <div className="findings ease">
                        <img src="/img/ease.png" alt="ease" />
                        <div className="findings-name">
                            <h3>Integrate with Ease</h3>
                            <p>Scan reports are machine readable and let you ingest them into your other systems. Be it vulnerability management system or report aggregator, the world is your oyster.</p>
                        </div>
                    </div>
                    <div className="findings fast">
                        <img src="/img/fast.png" alt="fast" />
                        <div className="findings-name">
                            <h3>Blazing Fast </h3>
                            <p>Get results fast. OpalOPC is capable of scanning single targets in seconds and hundreds of servers in a couple of minutes.</p>
                        </div>
                    </div>
                    <div className="findings default">
                        <img src="/img/default.png" alt="default" />
                        <div className="findings-name">
                            <h3>Intuitive With Powerful Defaults</h3>
                            <p>No tiring configuration just to get any results. The defaults are incredibly effective. Just aim at a target and fire. Configuration still available for advanced users.</p>
                        </div>
                    </div>
                    <div className="findings report">
                        <img src="/img/report.png" alt="report" />
                        <div className="findings-name">
                            <h3>Simple & Easy To Understand Report</h3>
                            <p>See the security posture of your OPC UA servers at a single glance. OpalOPC ranks the findings for you and provides instructions for fixing them.</p>
                        </div>
                    </div>
                </div>
                <h4>Launch your first Security Scan now.</h4>
                  <a href="#starthere" className="primary-btn common-btns">Get started</a>
            </div>
            
        </div>
    </div>
  );
};

export default Critical;
