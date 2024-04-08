import React from 'react';
import clsx from 'clsx';
export default function FooterLayout({ style, links, logo, copyright }) {
  return (
    <footer
      className={clsx('footer', {
        'footer--dark': style === 'dark',
      })}>
      <div className="container container-fluid">

        {links}
        {(logo || copyright) && (
          <div className="footer__bottom text--center">
            {logo && <div className="margin-bottom--sm">{logo}</div>}
            {copyright}
          </div>
        )}
      </div>

      {/* ### 3. Add Cookiebot cookie declaration to the footer of very page */}
      <script
        id="CookieDeclaration"
        src="https://consent.cookiebot.com/9625f1c9-907a-4cb8-95d0-15bfd9db488a/cd.js"
        type="text/javascript"
      ></script>

    </footer>
  );
}
