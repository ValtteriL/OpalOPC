import React from 'react';

const ReportMain = () => {
  return (
    <div class="main-container">
        <div class="main-txt">
            <h1><span>Reveal Security Issues</span> in your Most Critical Systems</h1>
            <p>OPC UA security scanner</p>
            <div class="main-btns">
                <a href="#starthere" class="primary-btn common-btns">Get started</a>
                <a href="/docs" class="secondary-btn common-btns">Read Docs</a>
            </div>
            <ul>
                <li><span></span>From download to testing report in <b>&lt;5min</b></li>
                <li><span></span>State of the art security, even with <b>zero OPC UA knowledge</b></li>
                <li><span></span>Say goodbye to unactionable results - <b>clear</b> prioritization &amp; fixing plan</li>
            </ul>
            <div class='dwnld-links'>
                <a href="#starthere" class="microsoft-link"><img src="/img/microsoft.jpg" alt="Microsoft" /></a>
                <a href="#starthere" class="snap-link"><img src="/img/snap.jpg" alt="Snap" /></a>
            </div>
        </div>
        <div class="main-img">
            <img src="/img/hero-image.png" alt="hero" />
        </div>
    </div>
  );
};

export default ReportMain;
