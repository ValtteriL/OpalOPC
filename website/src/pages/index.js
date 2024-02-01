import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import ReportMain from '../components/ReportMain/ReportMain';
import Testimonials from '../components/Testimonials/Testimonials';
import Critical from '../components/CriticalFindings/CriticalFinding';
import Featured from '../components/Featured/Featured';
import Price from '../components/Pricing/Pricing';
import Head from '@docusaurus/Head';

import Heading from '@theme/Heading';
import styles from './index.module.css';

function HomepageHeader() {
  // const {siteConfig} = useDocusaurusContext();
  return (
    <div>
      <HomepageHeader />
    </div>
    // <header className={clsx('hero hero--primary', styles.heroBanner)}>
    //   <div className="container">
    //     
    //     <Heading as="h1" className="hero__title">
    //       {siteConfig.title}
    //     </Heading>
    //     <p className="hero__subtitle">{siteConfig.tagline}</p>
    //     <div className={styles.buttons}>
    //       <Link
    //         className="button button--secondary button--lg"
    //         to="/docs">
    //         Docusaurus Tutorial - 5min ⏱️
    //       </Link>
    //     </div>
    //   </div>
    // </header>
  );
}

// Create component that adds og:image for frontpage
// https://docusaurus.io/docs/next/docusaurus-core#head
const MySEO = () => (
  <Head>
    <meta property='keywords' content='opc ua, security, automation, vulnerability scanner, application' />
  </Head>
);

export default function Home() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={`OPC UA Vulnerability Scanner`}
      description={`${siteConfig.tagline}`}
      >
        <MySEO />
      <main>
        <ReportMain />
        <Testimonials />
        <Critical />
        <Featured />
        <Price />
      </main>
    </Layout>
  );
}
