import React from 'react';

const ReportMain = () => {
  return (
    <div className="main-container">
        <div className="main-txt">
            <h1><span>Reveal Security Issues</span> in your Most Critical Systems</h1>
            <p>OPC UA security scanner</p>
            <div className="main-btns">
                <a href="/docs/get-started/quick-start" className="primary-btn common-btns">Get started</a>
                <a href="/docs" className="secondary-btn common-btns">Read Docs</a>
            </div>
            <ul>
                <li><span></span>From download to testing report in <b>&lt;5min</b></li>
                <li><span></span>State of the art security, even with <b>zero OPC UA knowledge</b></li>
                <li><span></span>Say goodbye to unactionable results - <b>clear</b> prioritization &amp; fixing plan</li>
            </ul>
            <div className='dwnld-links'>
                <a target="_blank" href="https://apps.microsoft.com/detail/OpalOPC/9N89VWR0GK7H?launch=true&mode=mini" className="microsoft-link"><img src="/img/microsoft.jpg" alt="Microsoft" /></a>
                <a target="_blank" href="https://snapcraft.io/opalopc" className="snap-link"><img src="/img/snap.jpg" alt="Snap" /></a>
            </div>
        </div>
        <div className="main-img">
            <img src="/img/hero-image.png" alt="hero" />
        </div>
    </div>
  );
};

export default ReportMain;
