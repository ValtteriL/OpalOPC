import React from 'react';

const ReportMain = () => {
  return (
    <div className="main-container">
        <div className="main-txt">
            <h1><span>Reveal Security Issues</span> in your Most Critical Systems</h1>
            <p>OPC UA security scanner</p>
            <div className="main-btns">
                <a href="#starthere" className="primary-btn common-btns">Get started</a>
                <a href="/docs" className="secondary-btn common-btns">Read Docs</a>
            </div>
            <ul>
                <li><span></span>From download to testing report in <b>&lt;5min</b></li>
                <li><span></span>State of the art security, even with <b>zero OPC UA knowledge</b></li>
                <li><span></span>Say goodbye to unactionable results - <b>clear</b> prioritization &amp; fixing plan</li>
            </ul>
            <div className='dwnld-links'>
                <a href="#starthere" className="microsoft-link"><img src="/img/microsoft.jpg" alt="Microsoft" /></a>
                <a href="#starthere" className="snap-link"><img src="/img/snap.jpg" alt="Snap" /></a>
            </div>
        </div>
        <div className="main-img">
            <img src="/img/hero-image.png" alt="hero" />
        </div>
    </div>
  );
};

export default ReportMain;
