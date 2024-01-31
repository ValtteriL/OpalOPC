import React from 'react';

const Testimonials = () => {
  return (
    <div class="testimonials-container">
        <div class="common-container">
            <div class="tst-wrapper">
                <div class="tst-title">
                    <div class="tst-name">
                        <h3>Lassi Korhonen</h3>
                        <p>white hat hacker</p>
                    </div>
                    <img src="/img/client.png" alt="client" />
                </div>
                
                <div class='tst-txt'>
                    <img src="/img/quote.svg" alt="quote" />
                    <p>OpalOPC saves me time and effort by automating OPC security tests. It’s easy to use and doesn’t require any OPC-specific knowledge, so it’s a no-brainer for OT security assessments.</p>
                </div>
            </div>
            
        </div>
    </div>
  );
};

export default Testimonials;