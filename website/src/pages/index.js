import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import ReportMain from '../components/ReportMain/ReportMain';
import Testimonials from '../components/Testimonials/Testimonials';
import Critical from '../components/CriticalFindings/CriticalFinding';
import Featured from '../components/Featured/Featured';
import Head from '@docusaurus/Head';

import Heading from '@theme/Heading';
import styles from './index.module.css';

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
      </main>
    </Layout>
    
  );
}

