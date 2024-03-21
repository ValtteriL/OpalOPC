import React from 'react';
import Footer from '@theme-original/Footer';
import Head from '@docusaurus/Head';

// Add mailchimp popup form script to the head of all pages
// head couldnt be directly swizzled, so swizzling footerwrapper in hopes that all pages use it
// adding according to react-helmet: https://www.npmjs.com/package/react-helmet
const MyHead = () => (
  <Head>
    <script id="mcjs">{`
      !function(c,h,i,m,p){m=c.createElement(h),p=c.getElementsByTagName(h)[0],m.async=1,m.src=i,p.parentNode.insertBefore(m,p)}(document,"script","https://chimpstatic.com/mcjs-connected/js/users/b8481fa110fd7bae3191c7b5f/c89d851cebe90d14cf64730b7.js");
    `}</script>
  </Head>
);

export default function FooterWrapper(props) {
  return (
    <>
    <MyHead />
      <Footer {...props} />
    </>
  );
}
