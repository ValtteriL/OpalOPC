---
slug: iec-62443-4-1-2-and-opc-ua
title: How OpalOPC helps you automate SVV for IEC 62443
keywords: [iec 62443-4-1, iec 62443-4-2, isa/iec 62443]
authors: [valtteri]
tags: [ISA/IEC 62443, SDLC, certification]
---

You are the producer of a component with an OPC UA interface. You want to certify it to IEC 62443-4-2 using ISASecure [CSA](https://isasecure.org/certification/iec-62443-csa-certification) or [ICSA](https://isasecure.org/certification/iec-62443-icsa-certification) schemes.

Before you can do that, you must certify its development process to IEC 62443-4-1 using ISASecure [SDLA](https://isasecure.org/certification/iec-62443-sdla-certification). To achieve the SDLA certificate, you must demonstrate that you follow the 8 security practices defined in the [IEC 62443-4-1 standard](https://webstore.iec.ch/publication/33615).

Security Verification and Validation (SVV) is one of those 8 practices. SVV is expensive to execute manually, as it requires specialized knowledge and consumes a lot of time. Automation can help make it more affordable.

In this post, I explore the extent the SVV practice for the OPC UA interface can be automated and sped up with the help of OpalOPC.

<!-- truncate -->

## Security Verification and Validation (SVV)

The purpose of SVV is to verify that your product security functions meet the security requirements and that the product handles error scenarios and invalid input correctly.

It contains the following requirements:

1. Security requirements testing (SVV-1)
2. Threat mitigation testing (SVV-2)
3. Vulnerability testing (SVV-3)
4. Penetration testing (SVV-4)
5. Independence of testers (SVV-5)

Out of these, SVV-1, SVV-2, SVV-3, and SVV-4 have content that can be automated.

### Security requirements testing (SVV-1)

There shall be functional tests for the applicable security requirements, performance, and scalability tests, as well as Boundary/edge condition, stress, and malformed or unexpected input tests.

The security requirements for different types of products are defined in SR-3 Product security requirements in practice 2 - Specification of security requirements (SR). OPC Foundation has created [a mapping](https://reference.opcfoundation.org/Core/Part2/v105/docs/A) of the requirements that apply to OPC UA.

The tests required in SVV-1 are not covered by OpalOPC. A possible solution could be a suite of functional tests tailored for the product under development.

### Threat mitigation testing (SVV-2)

A process shall be employed for testing the effectiveness of the mitigation for the threats identified and validated in the threat model. Activities shall include:

1. creating and executing plans to ensure that each mitigation implemented to address a specific threat has been adequately tested to ensure that the mitigation works as designed; and
1. creating and executing plans for attempting to thwart each mitigation.

The tests required in SVV-2 are product-specific and depend on a threat model. Therefore, it is not possible to say how to automate it. OpalOPC can potentially be part of a solution.

### Vulnerability testing (SVV-3)

A process shall be employed for performing tests that focus on identifying and characterizing potential security vulnerabilities in the product. Known vulnerability testing shall be based upon, at a minimum, recent contents of an established, industry-recognized, public source for known vulnerabilities. Testing shall include:

1. abuse case or malformed or unexpected input testing focused on uncovering security issues.
1. attack surface analysis to determine all avenues for ingress and egress to and from the system.
1. black box known vulnerability scanning focused on detecting known vulnerabilities.
1. software composition analysis (for compiled software).
1. dynamic runtime resource management testing.

Of these, black box known vulnerability scanning is fully covered by OpalOPC. This is thanks to the [Known vulnerability plugin](/docs/plugin-10017).

### Penetration testing (SVV-4)

SVV-4 requires that a process is employed to identify and characterize security-related issues via tests that focus on discovering and exploiting security vulnerabilities in the product.

OpalOPC allows penetration testers to get an in-depth view of OPC UA security in seconds in an automated fashion. It allows testers unfamiliar with OPC UA to conduct tests that otherwise have been completely omitted.

## Conclusion

The results of the exploration are visible in the table below:

| Requirement | Covered by OpalOPC |
| --- | :---: |
|Security requirements testing (SVV-1)|No|
|Threat mitigation testing (SVV-2)|Potentially|
|Vulnerability testing (SVV-3)|Partly|
|Penetration testing (SVV-4)|Yes|
|Independence of testers (SVV-5)|N/A|

OpalOPC can speed up the SVV practice by automating parts of it. The biggest impact it has on SVV-4, which it can potentially cover end-to-end. It also covers a key part of SVV-3. Finally, it can potentially help automate SVV-2.

In addition to the speedup, OpalOPC provides a deeper analysis of OPC UA security. This helps you catch more security defects.
