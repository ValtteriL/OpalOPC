import React from 'react';

const Featured = () => {
  return (
    <div className="featured-container">
        <div className="">
            <div className="featured-wrapper">
                <h2>featured on</h2>
                <section>
                    <article>
                        <div>
                        <ul>
                            <li><img src="/img/opc.png" alt="opc" /></li>
                            <li><img src="/img/isssource.png" alt="isssource" /></li>
                            <li><img src="/img/SecurityWeek.png" alt="SecurityWeek" /></li>
                            <li><img src="/img/Digital.png" alt="Digital" /></li>
                            <li><img src="/img/stackoverflow.png" alt="stackoverflow" /></li>
                            <li><img src="/img/hacker.png" alt="hacker" /></li>
                        </ul>
                        </div>
                        <div>
                        <ul>
                        <li><img src="/img/opc.png" alt="opc" /></li>
                            <li><img src="/img/isssource.png" alt="isssource" /></li>
                            <li><img src="/img/SecurityWeek.png" alt="SecurityWeek" /></li>
                            <li><img src="/img/Digital.png" alt="Digital" /></li>
                            <li><img src="/img/stackoverflow.png" alt="stackoverflow" /></li>
                            <li><img src="/img/hacker.png" alt="hacker" /></li>
                        </ul>
                        </div>
                    </article>
                </section>
                <div className="feat-imgs">
                    <img src="/img/opc.png" alt="opc" className='opc' />
                    <img src="/img/isssource.png" alt="isssource" className="isssource"/>
                    <img src="/img/SecurityWeek.png" alt="SecurityWeek" className="SecurityWeek" />
                    <img src="/img/Digital.png" alt="Digital" className="Digital" />
                    <img src="/img/stackoverflow.png" alt="stackoverflow" className="stackoverflow" />
                    <img src="/img/hacker.png" alt="hacker" className="hacker" />
                </div>
            </div>
        </div>
    </div>
  );
};

export default Featured;