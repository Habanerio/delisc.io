import { Metadata } from 'next';

import { LinkResults } from '@/components/LinkResults';
import { TagCloud } from '@/components/tags/TagCloud';
import { WithRightSidebar } from '@/components/layouts/WithRightSidebar';

export const metadata: Metadata = {
   title: 'Delisc.io',
   description: 'A del.icio.us-ly "aspired" Web App',
};

export default function HomePage({
   searchParams,
}: {
   searchParams?: { [key: string]: string | string[] | undefined };
}) {
   return (
      <WithRightSidebar
         main={<LinkResults searchParams={searchParams} />}
         sidebar={<TagCloud maxTags={150} />}
      />
   );
}
