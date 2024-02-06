import React from 'react';

const Price = () => {
  return (
    <div class="price-container">
        <div class="common-container">
            <div class="price-wrapper">
                <div class="price">
                    <h2>Free</h2>
                    <h3>$0<span>/month</span></h3>
                    <p>(For non-commercial use only)</p>
                    <ul>
                        <li>GUI and CLI scanners</li>
                        <li>Windows and Linux support</li>
                        <li>Latest security tests</li>
                        <li>Automatic updates</li>
                        <li>Access to vulnerability database</li>
                    </ul>
                    <a href="/docs/get-started/quick-start" class="secondary-btn common-btns">Get Started</a>
                </div>
                <div class="price paid">
                    <h2>Paid</h2>
                    <h3>$239<span>/month</span></h3>
                    <p>(For commercial use)</p>
                    <ul>
                        <li>Everything in Free, plus:</li>
                        <li>Priority email support</li>
                    </ul>
                    <a target="_blank" href="https://buy.stripe.com/7sIeVt9J5fiV6SQbII" class="primary-btn common-btns">Get Started</a>
                </div>
            </div>
        </div>
    </div>
  );
};

export default Price;