export const WithRightSidebar = ({
   main,
   sidebar,
}: {
   main: React.ReactNode;
   sidebar: React.ReactNode;
}) => {
   return (
      <div>
         <div className='flex flex-col lg:flex-row'>
            <div className='w-full lg:w-3/4 lg:pr-4'>{main}</div>
            <div className='hidden lg:block lg:w-1/4 lg:pl-4'>
               <div className='sticky top-0'>
                  <div className='bg-white shadow-sm rounded-lg p-4 mb-4'>{sidebar}</div>
               </div>
            </div>
         </div>
      </div>
   );
};
