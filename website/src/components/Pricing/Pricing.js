import React from 'react';

const Price = () => {
  return (
    <div id="starthere" class="price-container">
        <div class="common-container">
            <div class="price-wrapper">
                <div class="price">
                    <h2>Paid</h2>
                    <h3>$239<span>/month</span></h3>
                    <ul>
                        <li>GUI and CLI scanners</li>
                        <li>Windows and Linux support</li>
                        <li>Latest security tests</li>
                        <li>Automatic updates</li>
                        <li>Access to vulnerability database</li>
                        <li>Email support</li>
                    </ul>
                      <a target="_blank" href="https://buy.stripe.com/7sIeVt9J5fiV6SQbII" class="secondary-btn common-btns">Buy now</a>
                </div>
                <div class="price paid">
                    <h2>Trial</h2>
                    <h3>Free</h3>
                    <ul>
                        <li>Everything in Paid</li>
                        <li>No credit card required</li>
                    </ul>
                      <a href="https://mailchi.mp/opalopc/trial" class="primary-btn common-btns">Start 14-day trial</a>
                </div>
            </div>
        </div>
    </div>
  );
};

export default Price;
